using Serilog;

class Services
{
    public void Run(WebApplication app)
    {
        ConfigureShopService(app);
        app.Run();
    }

    private void ConfigureShopService(WebApplication app)
    {
        app.MapGet("/products", () =>
        {
            Log.Information("/products");

            return Consts.GetProducts();
        })
        .WithName("Products")
        .WithOpenApi();

        //type ItemsRequest map[string]int
        app.MapPost("/checkout", async (CheckoutRequest req) =>
        {
            Log.Information("/checkout by {0}", req.username);
            foreach (var item in req.cartItems)
            {
                if (item.Value > 0)
                {
                    Log.Information("{0} bought productid:{1}, product:{2}, quantity:{3}", req.username, item.Key, Consts.GetProductName(item.Key), item.Value);
                }
            };
            // TODO: ship the products
            return await Task.FromResult(Results.Json(new RestResult("Success", "Thank you for purchasing!", "")));
        })
       .WithName("Checkout")
       .WithOpenApi();
    }
}
