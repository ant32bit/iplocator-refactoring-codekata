using System.Text.Json;
using IPLocator.Models;
using IPLocator.Models.Exceptions;
using IPLocator.Settings;
using Microsoft.Extensions.Options;

namespace IPLocator.Services;

public class IpApiService
{
    private readonly IOptions<ApiSettings> _hosts;

    public IpApiService(IOptions<ApiSettings> settings)
    {
        _hosts = settings;
    }
    
    public async Task<IpApiResponse> GetIpDetails(IpAddress ip, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{_hosts.Value.IpApi}/json/{ip.Value}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var ipDetails = JsonSerializer.Deserialize<IpApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false
            });

            if (ipDetails == null)
                throw new ApiException("API response was null");

            if ("fail".Equals(ipDetails.Status, StringComparison.OrdinalIgnoreCase))
                throw new ApiException("API failed to get details");
            
            return ipDetails;
        }
        else
        {
            throw new ApiException("API returned a non successful status code");
        }
    }
}

