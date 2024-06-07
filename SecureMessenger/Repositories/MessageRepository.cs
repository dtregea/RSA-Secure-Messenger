using MongoDB.Bson;
using MongoDB.Driver;
using SecureMessenger.Contexts;
using SecureMessenger.Models;

namespace SecureMessenger.Repositories;

public class MessageRepository
{
    private readonly IMongoCollection<Message> _messages;

    public MessageRepository(MongoDBContext context)
    {
        _messages = context.GetCollection<Message>("Messages");
    }

    public List<Message> GetMessagesByUsername(string username)
    {
        return _messages.Find(message => message.User.Username == username).ToList();
    }

    public Message CreateMessage(Message message)
    {
        _messages.InsertOne(message);
        return message;
    }
}