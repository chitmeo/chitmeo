namespace ChitMeo.Module.Auth.Application.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
}
