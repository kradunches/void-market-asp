using MongoDB.Driver;
using UserService.Models;

namespace UserService.Services;

public class UserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
        var database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
        _users = database.GetCollection<User>(configuration["MongoDB:CollectionName"]);
    }

    public async Task<List<User>> GetAllAsync() =>
        await _users.Find(u => true).ToListAsync();

    public async Task<User> GetByIdAsync(string id) =>
        await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(User user) =>
        await _users.InsertOneAsync(user);

    public async Task UpdateAsync(string id, User userIn) =>
        await _users.ReplaceOneAsync(u => u.Id == id, userIn);

    public async Task DeleteAsync(string id) =>
        await _users.DeleteOneAsync(u => u.Id == id);
}