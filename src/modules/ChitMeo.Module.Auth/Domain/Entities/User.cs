namespace ChitMeo.Module.Auth.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string GoogleId { get; set; } = default!;
}
