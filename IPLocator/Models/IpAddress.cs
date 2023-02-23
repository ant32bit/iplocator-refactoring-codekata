namespace IPLocator.Models;

public class IpAddress
{
    public static bool TryParse(string ip, out IpAddress ipAddress)
    {
        try
        {
            ipAddress = new IpAddress(ip);
            return true;
        }
        catch (Exception)
        {
            ipAddress = null;
            return false;
        }
    }
    
    public string Value { get; }
    public byte[] Bytes { get; }
    public uint Bits { get; }
    
    public IpAddress(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            throw new FormatException("not a valid IP address");
        }

        var ipBytes = new byte[4];
        var f = true;
        var i = 0;
        foreach (var c in ip)
        {
            if (f)
            {
                if (c is >= '0' and <= '9')
                {
                    ipBytes[i] = (byte)(c - '0');
                    f = false;
                }
                else
                {
                    throw new FormatException("not a valid IP address");
                }
            }
            else {  
                if (c is >= '0' and <= '9')
                {
                    if (ipBytes[i] == 0)
                        throw new FormatException("not a valid IP address");
                    var val = ipBytes[i] * 10 + (c - '0');
                    if (val > 255)
                        throw new FormatException("not a valid IP address");
                    ipBytes[i] = (byte)val;
                }
                else if (c == '.')
                {
                    i++;
                    f = true;
                    if (i == 4)
                        throw new FormatException("not a valid IP address");
                }
                else
                {
                    throw new FormatException("not a valid IP address");
                }
            }
        }

        if (f || i < 3)
            throw new FormatException("not a valid IP address");
        
        uint ipBits = ipBytes.Aggregate<byte, uint>(0, (ipBitArray, ipByte) => (ipBitArray << 8) + ipByte);

        Value = ip;
        Bytes = ipBytes;
        Bits = ipBits;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (obj is IpAddress ip)
            return ip.Bits == this.Bits;

        return false;
    }

    public override int GetHashCode()
    {
        return unchecked((int)Bits);
    }

    public override string ToString()
    {
        return Value;
    }
}