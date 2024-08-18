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

    [JsonPropertyName("products")]
    public Product[] Products { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public Order(string title)
    {
        Title = title;
        Products = new Product[] { };
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }
}
