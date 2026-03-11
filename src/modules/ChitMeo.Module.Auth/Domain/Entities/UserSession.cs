namespace ChitMeo.Module.Auth.Domain.Entities;

public class UserSession
{
    /// <summary>
    /// Id is the unique identifier of the user session, which can be used to track and manage user sessions. This is useful for security purposes, such as invalidating a specific session when the user logs out or when suspicious activities are detected. It can also be used for auditing purposes, such as tracking the login history of a user or analyzing the usage patterns of the application.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// UserId is the foreign key to the User entity, which represents the user who owns this session. This is useful for security purposes, such as invalidating all sessions of a user when their password is changed or their account is deactivated.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// User agent string of the client, which can be used to identify the device and browser of the user. This is useful for security purposes, such as detecting suspicious login activities or providing better user experience by showing the device and browser information in the user's account settings.
    /// </summary>
    public string UserAgent { get; set; } = default!;
    /// <summary>
    /// Device information extracted from the user agent string, such as "iPhone 12 Pro Max" or "Dell XPS 15". This is useful for security purposes, such as detecting suspicious login activities or providing better user experience by showing the device information in the user's account settings.
    /// </summary>
    public string Device { get; set; } = default!;
    /// <summary>
    /// IP address of the client, which can be used to identify the location of the user. This is useful for security purposes, such as detecting suspicious login activities or providing better user experience by showing the location information in the user's account settings.
    /// </summary>
    public string IPAddress { get; set; } = default!;
    /// <summary>
    /// The date and time when the session was created. This is useful for security purposes, such as detecting suspicious login activities or providing better user experience by showing the session creation time in the user's account settings.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    public DateTime LastActiveAt { get; set; }
}
