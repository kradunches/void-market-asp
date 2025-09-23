namespace UserService;

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserCreateDto
{
    public string Email { get; set; }
    public string Name { get; set; }
}