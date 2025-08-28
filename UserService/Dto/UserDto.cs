namespace UserService;

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}

public class UserCreateDto
{
    public string Email { get; set; }
    public string Name { get; set; }
}