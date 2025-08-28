using AutoMapper;
using MongoDB.Bson;
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

    public async Task<List<UserDto>> GetAllAsync()
    {
        var userModels = await _users.Find(u => true).ToListAsync();

        return _mapper.Map<List<UserDto>>(userModels);
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;
        var userModel = await _users.Find(u => u.Id == objectId).FirstOrDefaultAsync();
        
        return _mapper.Map<UserDto>(userModel);
    }

    public async Task<User> CreateAsync(UserCreateDto user)
    {
        var userModel = _mapper.Map<User>(user);
        userModel.Name = user.Name;
        userModel.Email = user.Email;
        userModel.CreatedAt = DateTime.UtcNow;
        userModel.UpdatedAt = DateTime.UtcNow;
        await _users.InsertOneAsync(userModel);
        return userModel;
    }

    public async Task<bool> UpdateAsync(string id, UserCreateDto userDto)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;

        var update = Builders<User>.Update
            .Set(u => u.Name, userDto.Name)
            .Set(u => u.Email, userDto.Email)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var result = await _users.UpdateOneAsync(u => u.Id == objectId, update);

        return result.MatchedCount == 1;

        // var user = await _users.Find(u => u.Id == objectId).FirstOrDefaultAsync();
        //
        // if (user == null)
        //     return false;
        //
        // var updatedUser = _mapper.Map<User>(userDto);
        //
        // updatedUser.Name = userDto.Name;
        // updatedUser.Email = userDto.Email;
        // updatedUser.UpdatedAt = DateTime.UtcNow;
        //
        // await _users.ReplaceOneAsync(u => u.Id == objectId, updatedUser);
        //
        // return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return false;
        
        var result = await _users.DeleteOneAsync(u => u.Id == objectId);

        return result.DeletedCount == 1;
    }
}