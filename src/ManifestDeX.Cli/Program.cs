using ManifestDeX.Cli.Bootstrapper;
using ManifestDeX.Cli.Presentation;
using Spectre.Console.Cli;

var services = ServiceRegistration.CreateServiceCollection();
var registrar = new SpectreTypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(SpectreCliConfigurator.Configure);

return app.Run(args);
