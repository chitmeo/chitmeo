namespace ChitMeo.Module.Auth.Domain.Entities;

public class ExternalLogin
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = default!;
    /// <summary>
    /// subject (sub) claim from the provider, which is the unique identifier of the user in the provider's system.
    /// </summary>
    public string ProviderUserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
