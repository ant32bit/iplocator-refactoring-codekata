using System.Net;
using IPLocator.Models;

namespace IPLocatorTests.Tests;

[TestFixture]
public class IpAddressTests
{
    [Test, Category("IP Address")]
    [TestCaseSource(nameof(ValidIps))]
    public void ValidIps_CreateIpAddressObjects(string ip, uint bits, byte b0, byte b1, byte b2, byte b3)
    {
        var obj = new IpAddress(ip);
        
        Assert.That(obj.Value, Is.EqualTo(ip));
        Assert.That(obj.Bits, Is.EqualTo(bits));
        Assert.That(obj.Bytes, Has.Length.EqualTo(4));
        Assert.That(obj.Bytes[0], Is.EqualTo(b0));
        Assert.That(obj.Bytes[1], Is.EqualTo(b1));
        Assert.That(obj.Bytes[2], Is.EqualTo(b2));
        Assert.That(obj.Bytes[3], Is.EqualTo(b3));
    }
    
    [Test, Category("IP Address")]
    [TestCaseSource(nameof(InvalidIps))]
    public void InvalidIps_ThrowFormatExceptions(string ip)
    {
        Assert.Throws<FormatException>(() =>
        {
            var obj = new IpAddress(ip);
        });
    }
    
    [Test, Category("IP Address")]
    [TestCaseSource(nameof(ValidIps))]
    public void TryParseValidIps_CreateIpAddressObjects(string ip, uint bits, byte b0, byte b1, byte b2, byte b3)
    {
        var result = IpAddress.TryParse(ip, out var obj);
        
        Assert.That(result, Is.True);
        Assert.That(obj.Value, Is.EqualTo(ip));
        Assert.That(obj.Bits, Is.EqualTo(bits));
        Assert.That(obj.Bytes, Has.Length.EqualTo(4));
        Assert.That(obj.Bytes[0], Is.EqualTo(b0));
        Assert.That(obj.Bytes[1], Is.EqualTo(b1));
        Assert.That(obj.Bytes[2], Is.EqualTo(b2));
        Assert.That(obj.Bytes[3], Is.EqualTo(b3));
    }
    
    [Test, Category("IP Address")]
    [TestCaseSource(nameof(InvalidIps))]
    public void TryParseInvalidIps_ReturnsFalse(string ip)
    {
        var result = IpAddress.TryParse(ip, out var obj);
        
        Assert.That(result, Is.False);
        Assert.That(obj, Is.Null);
    }

    [Test, Category("IP Address")]
    public void IpAddress_ToString()
    {
        var ipValue = "127.0.0.1";
        var ipAddress = new IpAddress(ipValue);
        
        Assert.That(ipAddress.ToString(), Is.EqualTo(ipValue));
    }

    [Test, Category("IP Address")]
    [TestCase("127.0.0.1", 2130706433)]
    [TestCase("192.168.0.1", -1062731775)]
    public void IpAddressHashes(string ip, int expectedHash)
    {
        var ipAddress = new IpAddress(ip);
        var ipHash = ipAddress.GetHashCode();
        
        Assert.That(ipHash, Is.EqualTo(expectedHash));
    }

    [Test, Category("IP Address")]
    public void TemporallyDifferentObjects_EqualsTrue()
    {
        var ipValue = "127.0.0.1";
        var ip1 = new IpAddress(ipValue);
        var ip2 = new IpAddress(ipValue);
        
        Assert.That(ip1.Equals(ip2), Is.True);
    }
    
    [Test, Category("IP Address")]
    [TestCaseSource(nameof(DifferentIpAddresses))]
    public void DifferentIpAddress_EqualsFalse(IpAddress ipA, object objB)
    {
        Assert.That(ipA.Equals(objB), Is.False);
    }

    public static object[] ValidIps =
    {
        new object[] {"127.0.0.1", (uint)0x7f000001, (byte)127, (byte)0, (byte)0, (byte)1},
        new object[] {"255.255.255.255", (uint)0xffffffff, (byte)255, (byte)255, (byte)255, (byte)255}
    };

    public static object[] InvalidIps =
    {
        new object[] {null},
        new object[] {""},
        new object[] {"256.0.0.1"},
        new object[] {"127.256.0.1"},
        new object[] {"127.0.256.1"},
        new object[] {"127.0.0.256"},
        new object[] {"127.0.0.-1"},
        new object[] {"127.0.0.00"},
        new object[] {"127.0.0.01"},
        new object[] {"127.0.0."},
        new object[] {"127.0.0"},
        new object[] {".0.0.1"},
        new object[] {".127.0.0.1"},
        new object[] {"127.0.0.1."},
        new object[] {"127.0.0.1a"},
        new object[] {"localhost"}
    };

    public static object[] DifferentIpAddresses =
    {
        new object[] { new IpAddress("127.0.0.1"), new IpAddress("192.168.0.1") },
        new object[] { new IpAddress("127.0.0.1"), null },
        new object[] { new IpAddress("127.0.0.1"), "127.0.0.1" }
    };
}