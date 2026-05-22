namespace V4H.Application.Common.Interfaces;

public interface IJwtService
{
    string Generate(Guid userId, string email, string role);
}
