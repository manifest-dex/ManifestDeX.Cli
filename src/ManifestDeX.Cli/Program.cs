using System.Text;
using ManifestDeX.Cli.Bootstrapper;
using ManifestDeX.Cli.Presentation;
using Spectre.Console.Cli;

Console.OutputEncoding = Encoding.UTF8;

var services = ServiceRegistration.CreateServiceCollection();
var registrar = new SpectreTypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(SpectreCliConfigurator.Configure);

return app.Run(args);
