namespace IPLocator.Models;

public class IpMask
{
    public string Value { get; }
    public IpAddress Ip { get; }
    public int Size { get; }
    
    public uint Mask { get; }
    
    public IpMask(string mask)
    {
        var maskComponents = mask.Split('/');
        if (maskComponents.Length != 2 || maskComponents[1].Length is < 1 or > 2)
            throw new FormatException("not a valid IP mask");
        
        int maskSize;
        if (maskComponents[1][0] is >= '0' and <= '9')
            maskSize = (byte)(maskComponents[1][0] - '0');
        else
            throw new FormatException("not a valid IP mask");

        if (maskComponents[1].Length == 2)
        {
            if (maskSize == 0)
                throw new FormatException("not a valid IP mask");
            
            if (maskComponents[1][1] is >= '0' and <= '9')
                maskSize = maskSize * 10 + (byte)(maskComponents[1][1] - '0');
            else
                throw new FormatException("not a valid IP mask");
        }

        if (maskSize > 32)
            throw new FormatException("not a valid IP mask");

        if (!IpAddress.TryParse(maskComponents[0], out var maskIp))
            throw new FormatException("not a valid IP mask");
        
        Value = mask;
        Ip = maskIp;
        Size = maskSize;
        
        Mask = maskIp.Bits >> (32 - maskSize);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (obj is IpMask mask)
            return mask.Mask == this.Mask && mask.Size == this.Size;

        return false;
    }

    public override int GetHashCode()
    {
        return unchecked((int)(Mask << (32 - Size)));
    }

    public override string ToString()
    {
        return Value;
    }
}