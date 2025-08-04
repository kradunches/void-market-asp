using AutoMapper;
using MongoDB.Driver;
using UserService.Models;

namespace UserService.Services;

public class UserRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly IMapper _mapper;

    public UserRepository(IMongoClient client, IConfiguration configuration, IMapper mapper)
    {
        var database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
        _users = database.GetCollection<User>(configuration["MongoDB:CollectionName"]);
        _mapper = mapper;
    }

    public async Task<List<User>> GetAllAsync() =>
        await _users.Find(u => true).ToListAsync();

    public async Task<User> GetByIdAsync(string id) =>
        await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User> CreateAsync(UserDto user)
    {
        var userModel = _mapper.Map<User>(user);
        userModel.Name = user.Name;
        userModel.Email = user.Email;
        userModel.CreatedAt = DateTime.UtcNow;
        userModel.UpdatedAt = DateTime.UtcNow;
        await _users.InsertOneAsync(userModel);
        return userModel;
    }

    public async Task<bool> UpdateAsync(string id, UserDto userDto)
    {
        var user = await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        if (user == null)
            return false;

        var updatedUser = _mapper.Map<User>(userDto);

        updatedUser.UpdatedAt = DateTime.UtcNow;

        await _users.ReplaceOneAsync(u => u.Id == id, updatedUser);

        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);

        return result.DeletedCount == 1;
    }
}