using SecureMessenger.Contexts;
using SecureMessenger.Models;
using MongoDB.Driver;

namespace SecureMessenger.Repositories;

public class UserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoDBContext context)
    {
        _users = context.GetCollection<User>("Users");
    }

    public User GetUserByName(string name)
    {
        return _users.Find<User>(user => user.Username == name).FirstOrDefault();
    }

    public User CreateUser(User user)
    {
        _users.InsertOne(user);
        return user;
    }
}