using System.IO.Compression;
using ManifestDeX.Cli.Application.Errors;
using ManifestDeX.Cli.Domain.Entities;
using ManifestDeX.Cli.Domain.Ports;

namespace ManifestDeX.Cli.Application.UseCases;

public sealed class DownloadManifestsUseCase
{
    private readonly IManifestDexApiClient _apiClient;

    public DownloadManifestsUseCase(IManifestDexApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task ExecuteAsync(
        uint? appId,
        List<uint>? depotIds,
        List<string>? manifestsRaw,
        string? outDir,
        bool saveAsZip,
        Action<string, double>? progressCallback,
        CancellationToken cancellationToken = default)
    {
        // 1. Validation Checks
        var hasAppId = appId.HasValue && appId > 0;
        var hasManifests = manifestsRaw != null && manifestsRaw.Count > 0;

        if (!hasAppId && !hasManifests)
        {
            throw new CliException("You must specify either an appId or the --manifests option.", CliExitCode.ValidationError);
        }

        if (hasAppId && hasManifests)
        {
            throw new CliException("You cannot specify both an appId and the --manifests option.", CliExitCode.ValidationError);
        }

        if (depotIds != null && depotIds.Count > 0 && !hasAppId)
        {
            throw new CliException("The --depots option can only be used when an appId is provided.", CliExitCode.ValidationError);
        }

        // 2. Resolve Output Target Directory
        string targetDir;
        if (!string.IsNullOrWhiteSpace(outDir))
        {
            targetDir = Path.GetFullPath(outDir);
        }
        else
        {
            targetDir = hasAppId
                ? Path.Combine(Directory.GetCurrentDirectory(), "manifests", appId.Value.ToString())
                : Path.Combine(Directory.GetCurrentDirectory(), "manifests");
        }

        try
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
        }
        catch (Exception ex)
        {
            throw new CliException($"Failed to create destination directory: {targetDir}", CliExitCode.ValidationError, ex);
        }

        // 3. Parse specific manifests if provided
        List<AvailableManifest>? resolvedManifests = null;
        if (hasManifests)
        {
            resolvedManifests = new List<AvailableManifest>();
            foreach (var raw in manifestsRaw!)
            {
                var parts = raw.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2 || !uint.TryParse(parts[0], out var depotId) || !ulong.TryParse(parts[1], out var manifestId))
                {
                    throw new CliException($"Invalid manifest pair format: '{raw}'. Expected 'depotId:manifestId' (e.g. 731:89273928172938).", CliExitCode.ValidationError);
                }
                resolvedManifests.Add(new AvailableManifest(depotId, manifestId, 0));
            }
        }

        // 4. Trigger server-side preparation
        progressCallback?.Invoke("Preparing manifest download on server...", 0);
        
        var response = await _apiClient.PrepareDownloadAsync(appId, depotIds, resolvedManifests, cancellationToken);
        if (!response.Success)
        {
            throw new CliException($"Server failed to prepare download: {response.Error}", CliExitCode.UnknownError);
        }

        // 5. Polling Loop if task is not completed instantly
        var status = response;
        if (status.Status == "pending" || status.Status == "processing")
        {
            var taskId = status.TaskId ?? throw new CliException("Server returned success but missing taskId.", CliExitCode.UnknownError);
            
            while (status.Status == "pending" || status.Status == "processing")
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Delay(2000, cancellationToken);
                
                status = await _apiClient.GetDownloadStatusAsync(taskId, cancellationToken);
                if (!status.Success)
                {
                    throw new CliException($"Server failed to poll download task: {status.Error}", CliExitCode.UnknownError);
                }

                var progressPercentage = status.ProgressPercentage;
                var progressText = string.IsNullOrWhiteSpace(status.ProgressText) ? "Processing..." : status.ProgressText;
                
                progressCallback?.Invoke($"Server task [{status.Status}]: {progressText}", progressPercentage);
            }
        }

        if (status.Status == "failed")
        {
            throw new CliException($"Server background processing failed: {status.Error}", CliExitCode.UnknownError);
        }

        var downloadUrl = status.DownloadUrl;
        if (string.IsNullOrWhiteSpace(downloadUrl))
        {
            throw new CliException("Server completed task successfully but did not provide a download URL.", CliExitCode.UnknownError);
        }

        // Print warning if there were partial download failures on the server
        if (!string.IsNullOrWhiteSpace(status.Error))
        {
            progressCallback?.Invoke($"WARNING from server: {status.Error}", 100);
        }

        // 6. Download the stream
        progressCallback?.Invoke("Downloading manifest file stream...", 100);
        
        using var stream = await _apiClient.DownloadStreamAsync(downloadUrl, cancellationToken);

        // Determine if target URL is a single manifest or zip
        var isZip = downloadUrl.Contains("/zip/") || downloadUrl.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        if (saveAsZip)
        {
            // Save directly as a ZIP archive
            var zipFileName = hasAppId ? $"{appId.Value}_manifests.zip" : "manifests.zip";
            var targetZipPath = Path.Combine(targetDir, zipFileName);

            progressCallback?.Invoke($"Saving zip archive directly to: {targetZipPath}", 100);

            if (File.Exists(targetZipPath))
            {
                try { File.Delete(targetZipPath); } catch { }
            }

            if (isZip)
            {
                using var fileStream = new FileStream(targetZipPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await stream.CopyToAsync(fileStream, cancellationToken);
            }
            else
            {
                // Manually wrap single raw manifest into a valid zip archive
                var rawFileName = Path.GetFileName(downloadUrl);
                if (string.IsNullOrWhiteSpace(rawFileName))
                {
                    rawFileName = "downloaded.manifest";
                }

                using (var zipStream = new FileStream(targetZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    var entry = archive.CreateEntry(rawFileName);
                    using var entryStream = entry.Open();
                    await stream.CopyToAsync(entryStream, cancellationToken);
                }
            }
        }
        else
        {
            if (isZip)
            {
                // Download zip, extract its files, and clean it up
                var tempZipPath = Path.Combine(targetDir, $"temp_download_{Guid.NewGuid():N}.zip");
                
                progressCallback?.Invoke("Downloading temporary zip archive for extraction...", 100);
                
                using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream, cancellationToken);
                }

                progressCallback?.Invoke("Extracting manifest files locally...", 100);
                
                try
                {
                    ZipFile.ExtractToDirectory(tempZipPath, targetDir, overwriteFiles: true);
                }
                catch (Exception ex)
                {
                    throw new CliException($"Failed to extract zip archive locally to: {targetDir}", CliExitCode.UnknownError, ex);
                }
                finally
                {
                    try
                    {
                        if (File.Exists(tempZipPath))
                        {
                            File.Delete(tempZipPath);
                        }
                    }
                    catch { /* ignore cleanup errors */ }
                }

                progressCallback?.Invoke("Extraction completed successfully.", 100);
            }
            else
            {
                // Download a single raw manifest file
                var fileName = Path.GetFileName(downloadUrl);
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = "downloaded.manifest";
                }
                
                var targetFilePath = Path.Combine(targetDir, fileName);
                
                progressCallback?.Invoke($"Saving manifest file directly to: {targetFilePath}", 100);
                
                using var fileStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await stream.CopyToAsync(fileStream, cancellationToken);
            }
        }
    }
}
