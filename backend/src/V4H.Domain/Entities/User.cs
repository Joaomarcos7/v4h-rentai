using V4H.Domain.Enums;

namespace V4H.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { }

    public static User Create(string name, string email, string passwordHash, UserRole role)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
