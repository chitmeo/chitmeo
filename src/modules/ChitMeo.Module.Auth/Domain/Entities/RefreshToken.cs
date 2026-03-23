namespace ChitMeo.Module.Auth.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    /// <summary>
    /// Hash token will be stored in database, while the original token will be sent to client.
    /// This is to prevent token theft from database.
    /// </summary>
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByIP { get; set; } = default!;
}
