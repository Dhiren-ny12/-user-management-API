using Microsoft.AspNetCore.Mvc;

// =====================================================
// UsersController.cs
// Full CRUD with validation
// Copilot assisted with initial structure and debugging
// =====================================================

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    // Inject UserService and Logger via DI (Dependency Injection)
    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // ── GET /api/users ──────────────────────────────
    // Returns all users
    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Fetching all users");
        var users = _userService.GetAll();
        return Ok(users);
    }

    // ── GET /api/users/{id} ─────────────────────────
    // Returns one user by ID
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        _logger.LogInformation("Fetching user with ID {Id}", id);

        var user = _userService.GetById(id);
        if (user is null)
        {
            _logger.LogWarning("User {Id} not found", id);
            return NotFound(new { error = $"User with ID {id} not found." });
        }

        return Ok(user);
    }

    // ── POST /api/users ─────────────────────────────
    // Creates a new user — includes validation
    [HttpPost]
    public IActionResult Create([FromBody] CreateUserDto dto)
    {
        // ── VALIDATION ──────────────────────────────
        // Copilot generated basic null checks; added email format and
        // duplicate email check based on business requirements

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { error = "Name is required." });

        if (dto.Name.Length < 2 || dto.Name.Length > 100)
            return BadRequest(new { error = "Name must be between 2 and 100 characters." });

        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { error = "Email is required." });

        if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
            return BadRequest(new { error = "Email format is invalid." });

        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { error = "Password is required." });

        if (dto.Password.Length < 6)
            return BadRequest(new { error = "Password must be at least 6 characters." });

        // Check if email already exists
        if (_userService.EmailExists(dto.Email))
            return Conflict(new { error = "A user with this email already exists." });

        // ── CREATE ──────────────────────────────────
        var createdUser = _userService.Create(dto);
        _logger.LogInformation("Created new user: {Name} (ID: {Id})", createdUser.Name, createdUser.Id);

        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    // ── PUT /api/users/{id} ─────────────────────────
    // Updates an existing user — includes validation
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateUserDto dto)
    {
        // ── VALIDATION ──────────────────────────────
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { error = "Name is required." });

        if (dto.Name.Length < 2 || dto.Name.Length > 100)
            return BadRequest(new { error = "Name must be between 2 and 100 characters." });

        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { error = "Email is required." });

        if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
            return BadRequest(new { error = "Email format is invalid." });

        // Check if new email is already taken by another user
        if (_userService.EmailExists(dto.Email, excludeId: id))
            return Conflict(new { error = "A user with this email already exists." });

        // ── UPDATE ──────────────────────────────────
        var updated = _userService.Update(id, dto);
        if (updated is null)
        {
            _logger.LogWarning("Update failed — user {Id} not found", id);
            return NotFound(new { error = $"User with ID {id} not found." });
        }

        _logger.LogInformation("Updated user {Id}", id);
        return Ok(updated);
    }

    // ── DELETE /api/users/{id} ──────────────────────
    // Deletes a user by ID
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var deleted = _userService.Delete(id);
        if (!deleted)
        {
            _logger.LogWarning("Delete failed — user {Id} not found", id);
            return NotFound(new { error = $"User with ID {id} not found." });
        }

        _logger.LogInformation("Deleted user {Id}", id);
        return NoContent();
    }
}
