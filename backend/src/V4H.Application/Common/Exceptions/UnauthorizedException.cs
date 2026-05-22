namespace V4H.Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Access denied.") : base(message) { }
}
