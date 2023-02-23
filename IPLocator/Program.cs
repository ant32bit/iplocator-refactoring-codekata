using IPLocator.Services;
using IPLocator.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.Configure<ApiSettings>(configuration.GetSection("Apis"));
services.AddTransient<IpApiService>();
services.AddTransient<IpLocatorService>();


var provider = services.BuildServiceProvider();

var application = provider.GetService<IpLocatorService>();

if (application != null)
{
    var cancellationTokenSource = new CancellationTokenSource();
    var cancellationToken = cancellationTokenSource.Token;

    await application.Start(cancellationToken);
}

