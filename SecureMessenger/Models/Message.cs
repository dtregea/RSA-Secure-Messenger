namespace SecureMessenger.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("User")]
    public User User { get; set; }
    
    [BsonElement("Sender")]
    public User Sender { get; set; }

    [BsonElement("Content")]
    public string Content { get; set; }
    
    [BsonElement("CreatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}