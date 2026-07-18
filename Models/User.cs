using System.Text.Json.Serialization;

// ── User Model ────────────────────────────────────
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "user";   // "user" or "admin"

    [JsonIgnore]  // Never expose password in API responses
    public string PasswordHash { get; set; } = "";
}

// ── DTO: What client sends when creating a user ──
// Copilot suggested using DTOs to separate input from the full model
public class CreateUserDto
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

// ── DTO: What client sends when updating a user ──
public class UpdateUserDto
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

// ── UserService: Business logic & in-memory store ─
public class UserService
{
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Name = "Alice Johnson", Email = "alice@nexbridge.com", Role = "admin" },
        new User { Id = 2, Name = "Bob Smith",     Email = "bob@nexbridge.com",   Role = "user"  },
        new User { Id = 3, Name = "Carol White",   Email = "carol@nexbridge.com", Role = "user"  }
    };

    private int _nextId = 4;

    public List<User> GetAll() => _users;

    public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

    public bool EmailExists(string email, int? excludeId = null) =>
        _users.Any(u => u.Email == email && u.Id != excludeId);

    public User Create(CreateUserDto dto)
    {
        var user = new User
        {
            Id = _nextId++,
            Name = dto.Name,
            Email = dto.Email,
            Role = "user",  // Role always set server-side, never by client
            PasswordHash = BCryptHash(dto.Password)
        };
        _users.Add(user);
        return user;
    }

    public User? Update(int id, UpdateUserDto dto)
    {
        var user = GetById(id);
        if (user is null) return null;
        user.Name = dto.Name;
        user.Email = dto.Email;
        return user;
    }

    public bool Delete(int id)
    {
        var user = GetById(id);
        if (user is null) return false;
        _users.Remove(user);
        return true;
    }

    // Simplified hash simulation (use BCrypt.Net in production)
    private static string BCryptHash(string password) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password + "_hashed"));
}
