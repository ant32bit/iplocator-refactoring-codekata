using IPLocator.Models;
using IPLocator.Models.Exceptions;
using IPLocator.Services;
using IPLocator.Settings;
using Microsoft.Extensions.Options;

namespace IPLocatorTests.Tests;

[TestFixture]
public class IpApiServiceTests
{
    [Test, Category("IP API Service")]
    public async Task ValidIp_GetsDetails()
    {
        var testIp = new IpAddress("147.161.212.100");
        var settings = Options.Create(new ApiSettings
        {
            IpApi = "http://ip-api.com"
        });
        
        var apiService = new IpApiService(settings);
        
        var result = await apiService.GetIpDetails(testIp);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.City, Is.EqualTo("Melbourne"));
        Assert.That(result.Region, Is.EqualTo("Victoria"));
        Assert.That(result.Country, Is.EqualTo("Australia"));
        Assert.That(result.Latitude, Is.EqualTo(-37.8372));
        Assert.That(result.Longitude, Is.EqualTo(144.9354));
    }
    
    [Test, Category("IP API Service")]
    public void ReservedIp_ThrowsApiException()
    {
        var reservedIp = new IpAddress("127.0.0.1");
        var settings = Options.Create(new ApiSettings
        {
            IpApi = "http://ip-api.com"
        });
        
        var apiService = new IpApiService(settings);

        Assert.ThrowsAsync<ApiException>(async () =>
        {
            var _ = await apiService.GetIpDetails(reservedIp);
        });
    }
    
    [Test, Category("IP API Service")]
    public void ServiceDown_ThrowsApiException()
    {
        var reservedIp = new IpAddress("147.161.212.100");
        var settings = Options.Create(new ApiSettings
        {
            IpApi = "http://google.com"
        });
        
        var apiService = new IpApiService(settings);

        Assert.ThrowsAsync<ApiException>(async () =>
        {
            var _ = await apiService.GetIpDetails(reservedIp);
        });
    }
}