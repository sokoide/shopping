using System.Text.Json.Serialization;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Order
{
    [JsonPropertyName("id")]
    [BsonId] // This attribute specifies that this property maps to the _id field in MongoDB
    public ObjectId Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("items")]
    public string Items { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public Order(string title, string items = "")
    {
        Title = title;
        Items = items;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }
}
