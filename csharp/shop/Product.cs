using System.Text.Json.Serialization;

class Product
{
    [JsonPropertyName("id")]
    public int ID { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("price")]
    public double Price { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; }

    public Product()
    {
        ID = 0;
        Title = "";
        Price = 0.0f;
        Description = "";
        Category = "";
        Image = ""; ;
    }

    public Product(int id, string title, double price, string description, string category, string image)
    {
        ID = id;
        Title = title;
        Price = price;
        Description = description;
        Category = category;
        Image = image;
    }
}