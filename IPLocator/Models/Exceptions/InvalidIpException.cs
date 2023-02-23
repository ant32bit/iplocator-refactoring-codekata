namespace IPLocator.Models.Exceptions;

public class InvalidIpException: Exception
{
    public InvalidIpException(string message) : base(message) { }
}