using System;

namespace ChitMeo.Module.Auth.Domain.Entities;

public class UserPassword
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    /// <summary>
    /// Password salt is a random string that is used to hash the password. It is stored in the database along with the password hash. When a user tries to log in, the system retrieves the salt from the database, combines it with the provided password, and hashes the result. This is done to prevent rainbow table attacks, where attackers precompute hashes for common passwords. By using a unique salt for each user, even if two users have the same password, their password hashes will be different, making it much harder for attackers to crack the passwords.
    /// </summary>
    public string PasswordSalt { get; set; } = default!;
    /// <summary>
    /// Password hash is the result of hashing the password combined with the salt. It is stored
    /// in the database instead of the original password to enhance security. When a user tries to log in, the system retrieves the salt from the database, combines it with the provided password, and hashes the result. If the resulting hash matches the stored password hash, the login is successful. This way, even if an attacker gains access to the database, they will not be able to retrieve the original passwords, as they only have access to the salted and hashed versions.
    /// </summary>
    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = true;

}
