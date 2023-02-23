using System.ComponentModel.DataAnnotations;
using IPLocator.Models.Exceptions;
using IPLocator.Services;
using IPLocator.Settings;
using Microsoft.Extensions.Options;

namespace IPLocatorTests.Tests;

[TestFixture]
public class IpLocatorServiceTests
{
    [Test, Category("IP Locator Service")]
    public async Task ValidIp_GetsValidDetails()
    {
        var settings = Options.Create(new ApiSettings
        {
            IpApi = "http://ip-api.com"
        });
        
        var apiService = new IpApiService(settings);
        var locatorService = new IpLocatorService(apiService);

        var (locationName, locationCoords) = await locatorService.EvaluateIp("147.161.212.100");
        
        Assert.That(locationName, Is.EqualTo("Melbourne, Victoria, Australia"));
        Assert.That(locationCoords, Is.EqualTo("-37.837, 144.935"));
    }
    
    [Test, Category("IP Locator Service")]
    public void MalformedIp_RaisesFormatException()
    {
        var settings = Options.Create(new ApiSettings { IpApi = "http://test.com" });
        var apiService = new IpApiService(settings);
        var locatorService = new IpLocatorService(apiService);

        Assert.ThrowsAsync<FormatException>(async () =>
        {
            var _= await locatorService.EvaluateIp("256.256.256.256");
        });
    }

    [Test, Category("IP Locator Service")]
    public void InvalidIp_RaisesInvalidIpException()
    {
        var settings = Options.Create(new ApiSettings { IpApi = "http://test.com" });
        var apiService = new IpApiService(settings);
        var locatorService = new IpLocatorService(apiService);

        Assert.ThrowsAsync<InvalidIpException>(async () =>
        {
            var _ = await locatorService.EvaluateIp("127.0.0.1");
        });
    }
    
    [Test, Category("IP Locator Service")]
    public void ApiIssues_RaiseApiException()
    {
        var settings = Options.Create(new ApiSettings { IpApi = "http://google.com" });
        var apiService = new IpApiService(settings);
        var locatorService = new IpLocatorService(apiService);

        Assert.ThrowsAsync<ApiException>(async () =>
        {
            var _ = await locatorService.EvaluateIp("147.161.212.100");
        });
    }
}