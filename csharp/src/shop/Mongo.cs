using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

public class Mongo
{
    const string ConnectionString = "mongodb://root:password@localhost/?retryWrites=true&w=majority";
    private string DB = "shop";

    static readonly Mongo instance = new Mongo();

    public static Mongo Instance { get { return instance; } }

    private Mongo() { }

    public void SetUnittest()
    {
        DB = "shop_unittest";
    }
    public void ResetUnittest()
    {
        DB = "shop";
    }

    private IMongoDatabase getDB()
    {
        var client = new MongoClient(ConnectionString);
        return client.GetDatabase(DB);
    }

    public Order? FindOrder(string title)
    {
        var collection = getDB().GetCollection<BsonDocument>("orders");
        var filter = Builders<BsonDocument>.Filter.Eq("Title", title);
        var document = collection.Find(filter).FirstOrDefault();

        if (document == null) return null;
        return BsonSerializer.Deserialize<Order>(document);
    }

    public void InsertOrder(Order order)
    {
        var collection = getDB().GetCollection<BsonDocument>("orders");
        collection.InsertOne(order.ToBsonDocument());
    }

    public void DeleteAllOrders()
    {
        var collection = getDB().GetCollection<BsonDocument>("orders");
        collection.DeleteMany(FilterDefinition<BsonDocument>.Empty);
    }

    public void DeleteOrders(string title)
    {
        var collection = getDB().GetCollection<BsonDocument>("orders");
        var filter = Builders<BsonDocument>.Filter.Eq("Title", title);
        collection.DeleteMany(filter);
    }
}
