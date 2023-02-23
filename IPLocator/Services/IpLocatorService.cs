using IPLocator.Models;
using IPLocator.Models.Exceptions;

namespace IPLocator.Services;

public class IpLocatorService
{
    private static readonly IpMask[] InvalidIps =
    {
        // Reserved IPs
        // https://en.wikipedia.org/wiki/Reserved_IP_addresses
        new ("0.0.0.0/8"),
        new ("192.0.2.0/24"),
        new ("192.88.99.0/24"),
        new ("198.51.100.0/24"),
        new ("203.0.113.0/24"),
        new ("224.0.0.0/4"),
        new ("240.0.0.0/4"),
        new ("255.255.255.255/32"),
        // Private IPs
        new ("100.64.0.0/10"),
        new ("127.0.0.0/8"),
        new ("169.254.0.0/16"),
        new ("172.16.0.0/12"),
        new ("192.0.0.0/24"),
        new ("192.168.0.0/16"),
        new ("198.18.0.0/16")
    };
    
    private static bool IsValid(IpAddress ipAddress)
    {
        foreach (var mask in InvalidIps)
        {
            var maskedIp = ipAddress.Bits >> (32 - mask.Size);
            if (maskedIp == mask.Mask)
                return false;
        }

        return true;
    }

    private readonly IpApiService _ipApi;

    public IpLocatorService(IpApiService ipApiService)
    {
        _ipApi = ipApiService;
    }
    
    public async Task Start(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Welcome to the IPLocator");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("What IP are you locating? ");
            var ipInput = Console.ReadLine();

            try
            {
                var (locationName, locationCoords) = await EvaluateIp(ipInput!, cancellationToken);
                Console.WriteLine($"This IP is in {locationName} ({locationCoords})");
            }
            catch (FormatException)
            {
                Console.WriteLine("Could not parse IP address");
            }
            catch (InvalidIpException)
            {
                Console.WriteLine("IP address is invalid");
            }
            catch (ApiException)
            {
                Console.WriteLine("No details were found for this IP address");
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong");
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.Write("Would you like to try another? [y/n] ");
                var answer = Console.ReadKey();
                Console.WriteLine("");

                if (answer.KeyChar == 'Y' || answer.KeyChar == 'y')
                    break;

                if (answer.KeyChar == 'N' || answer.KeyChar == 'n')
                    return;
            }
        }
    }

    public async Task<(string, string)> EvaluateIp(string ip, CancellationToken cancellationToken = default)
    {
        var ipAddress = new IpAddress(ip);

        if (!IsValid(ipAddress))
                throw new InvalidIpException("invalid ip");

        var ipDetails = await _ipApi.GetIpDetails(ipAddress, cancellationToken);

        var locationName = string.Join(", ",
            new[] { ipDetails.City, ipDetails.Region, ipDetails.Country }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
                
        var locationCoords = string.Join(", ", ipDetails.Latitude.ToString("N3"),
            ipDetails.Longitude.ToString("N3"));

        return (locationName, locationCoords);
    }
}