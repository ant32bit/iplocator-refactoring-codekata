namespace IPLocator.Models.Exceptions;

public class ApiException: Exception
{
    public ApiException(string message) : base(message) { }
}
