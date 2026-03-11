namespace ChitMeo.Module.Auth.Application.Configurations;

public class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Key { get; set; } = default!;
    public int ExpireDays { get; set; }
}
