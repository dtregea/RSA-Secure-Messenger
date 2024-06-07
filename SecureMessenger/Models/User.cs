namespace SecureMessenger.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = String.Empty;

    [Required(ErrorMessage = "Username is required.")]
    [BsonElement("Username")]
    public string Username { get; set; }
    
    [BsonElement("PublicKey")]
    public string? PublicKey { get; set; } = String.Empty;
}