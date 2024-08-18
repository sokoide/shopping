namespace shop.tests;

public class TestMongo : IDisposable
{
    const string title = "unittest order";

    public TestMongo()
    {
        Mongo.Instance.SetUnittest();
        Mongo.Instance.DeleteAllOrders();
    }

    public void Dispose()
    {
        Mongo.Instance.DeleteAllOrders();
        Mongo.Instance.ResetUnittest();
    }

    [Fact]
    public void TestInsertOrder()
    {
        Mongo.Instance.DeleteOrders(title);

        var order = new Order(title);
        var got = Mongo.Instance.FindOrder(title);
        Assert.Null(got);

        Mongo.Instance.InsertOrder(order);
        got = Mongo.Instance.FindOrder(title);
        Assert.Equal(title, got?.Title);
    }
}
