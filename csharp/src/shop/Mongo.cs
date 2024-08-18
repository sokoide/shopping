using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;

public class Mongo
{
    private const string ConnectionString = "mongodb://root:password@localhost/?retryWrites=true&w=majority";
    private string _databaseName = "shop";

    private static readonly Lazy<Mongo> _instance = new Lazy<Mongo>(() => new Mongo());

    public static Mongo Instance => _instance.Value;

    private Mongo() { }

    public void SetUnittestDatabase() => _databaseName = "shop_unittest";
    public void ResetProductionDatabase() => _databaseName = "shop";

    private IMongoDatabase GetDatabase()
    {
        var settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
        settings.ConnectTimeout = TimeSpan.FromSeconds(5);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
        settings.MaxConnectionPoolSize = 10;

        return new MongoClient(settings).GetDatabase(_databaseName);
    }

    private IMongoCollection<Order> GetOrdersCollection() => GetDatabase().GetCollection<Order>("orders");

    public Order? FindOrder(string title)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Title, title);
        return GetOrdersCollection().Find(filter).FirstOrDefault();
    }

    public void InsertOrder(Order order) => GetOrdersCollection().InsertOne(order);

    public void DeleteAllOrders() => GetOrdersCollection().DeleteMany(Builders<Order>.Filter.Empty);

    public void DeleteOrders(string title)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Title, title);
        GetOrdersCollection().DeleteMany(filter);
    }
}
