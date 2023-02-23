using IPLocator.Models;

namespace IPLocatorTests.Tests;

[TestFixture]
public class IpMaskTests
{
    [Test, Category("IP Mask")]
    [TestCase("127.0.0.1/0", "127.0.0.1", 0)]
    [TestCase("127.0.0.1/3", "127.0.0.1", 3)]
    [TestCase("127.0.0.1/8", "127.0.0.1", 8)]
    [TestCase("127.0.0.1/16", "127.0.0.1", 16)]
    [TestCase("127.0.0.1/22", "127.0.0.1", 22)]
    [TestCase("127.0.0.1/24", "127.0.0.1", 24)]
    [TestCase("127.0.0.1/32", "127.0.0.1", 32)]
    public void ValidMasks_CreateIpMaskObjects(string mask, string ip, int size)
    {
        var obj = new IpMask(mask);
        
        Assert.That(obj.Value, Is.EqualTo(mask));
        Assert.That(obj.Ip.Value, Is.EqualTo(ip));
        Assert.That(obj.Size, Is.EqualTo(size));
    }
    
    [Test, Category("IP Mask")]
    [TestCase("127.0.0.1")]
    [TestCase("127.0.0.1/")]
    [TestCase("127.0.0.0/a")]
    [TestCase("127.0.0.1/33")]
    [TestCase("127.0.0.1/00")]
    [TestCase("127.0.0.1/01")]
    [TestCase("127.0.0.1/1a")]
    [TestCase(@"127.0.0.1\8")]
    [TestCase("127.0.0/8")]
    public void InvalidMasks_ThrowFormatExceptions(string mask)
    {
        Assert.Throws<FormatException>(() =>
        {
            var obj = new IpMask(mask);
        });
    }
    
    [Test, Category("IP Mask")]
    public void IpMask_ToString()
    {
        var maskValue = "127.0.0.0/8";
        var mask = new IpMask(maskValue);
        
        Assert.That(mask.ToString(), Is.EqualTo(maskValue));
    }

    [Test, Category("IP Mask")]
    [TestCase("127.0.0.0/8", 2130706432)]
    [TestCase("192.168.0.0/16", -1062731776)]
    public void IpMaskHashes(string maskValue, int expectedHash)
    {
        var mask = new IpMask(maskValue);
        var maskHash = mask.GetHashCode();
        
        Assert.That(maskHash, Is.EqualTo(expectedHash));
    }

    [Test, Category("IP Mask")]
    public void TemporallyDifferentObjects_EqualsTrue()
    {
        var maskValue = "127.0.0.0/8";
        var mask1 = new IpMask(maskValue);
        var mask2 = new IpMask(maskValue);
        
        Assert.That(mask1.Equals(mask2), Is.True);
    }
    
    [Test, Category("IP Mask")]
    [TestCaseSource(nameof(DifferentIpAddresses))]
    public void DifferentIpMask_EqualsFalse(IpMask maskA, object objB)
    {
        Assert.That(maskA.Equals(objB), Is.False);
    }

    public static object[] DifferentIpAddresses =
    {
        new object[] { new IpMask("127.0.0.0/8"), new IpMask("127.0.0.0/16") },
        new object[] { new IpMask("127.0.0.0/16"), new IpMask("127.0.0.0/24") },
        new object[] { new IpMask("127.0.0.0/8"), new IpMask("192.168.0.0/8") },
        new object[] { new IpMask("0.127.0.0/16"), new IpMask("127.0.0.0/8") },
        new object[] { new IpMask("127.0.0.0/0"), new IpAddress("127.0.0.0") },
        new object[] { new IpMask("127.0.0.0/8"), null },
        new object[] { new IpMask("127.0.0.0/8"), "127.0.0.0/8" }
    };
}