using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly UserRepository _userRepository;

    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<UserDto>>> GetAll() =>
        await _userRepository.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();
        
        return user;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserCreateDto userDto)
    {
        var createdUserDto = await _userRepository.CreateAsync(userDto);
        
        return CreatedAtAction(nameof(GetAll), new { id = createdUserDto.Id }, createdUserDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UserCreateDto userDto)
    {
        var updated = await _userRepository.UpdateAsync(id, userDto);

        if (!updated) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();
        await _userRepository.DeleteAsync(id);
        
        return NoContent();
    }
}