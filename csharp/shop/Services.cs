using Serilog;

class Services
{
    public void Run(WebApplication app)
    {
        ConfigureShopService(app);
        ConfigureChaosMonkeyService(app);
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

        app.MapGet("/login", (string username) =>
        {
            Log.Information("/login {0}", username);

            int flagid = Globals.GetFlagId("login");
            AtomicBoolean flag = Globals.BreakFlags[flagid];

            var result = flag.Get() ? new RestResult("Failure", "Login service is broken", "") : new RestResult("Success", "", "");
            return Results.Json(result);
        })
        .WithName("Login")
        .WithOpenApi();
    }
    private void ConfigureChaosMonkeyService(WebApplication app)
    {
        ConfigureBreakHandlers("login", app);
    }

    private void ConfigureBreakHandlers(string feature, WebApplication app)
    {
        app.MapGet("/break/" + feature, () =>
        {
            int flagid = Globals.GetFlagId(feature);
            AtomicBoolean flag = Globals.BreakFlags[flagid];

            Log.Information("/break/" + feature);
            flag.Set(true);
            return Results.StatusCode(200);
        })
        .WithName("break/" + feature)
        .WithOpenApi();

        app.MapGet("/fix/" + feature, () =>
        {
            int flagid = Globals.GetFlagId(feature);
            AtomicBoolean flag = Globals.BreakFlags[flagid];

            Log.Information("/fix/" + feature);
            flag.Set(false);
            return Results.StatusCode(200);
        })
        .WithName("fix/" + feature)
        .WithOpenApi();

        app.MapGet("/status/" + feature, () =>
        {
            int flagid = Globals.GetFlagId(feature);
            AtomicBoolean flag = Globals.BreakFlags[flagid];

            Log.Information("/status/" + feature);
            bool value = flag.Get();
            if (value)
            {
                return Results.Ok("Broken");
            }
            else
            {
                return Results.Ok("Operational");
            }
        })
        .WithName("status/" + feature)
        .WithOpenApi();
    }
}
