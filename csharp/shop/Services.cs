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

            var result = Globals.BreakLogin.Get() ? new RestResult("Failure", "Login service is broken", "") : new RestResult("Success", "", "");
            return Results.Json(result);
        })
        .WithName("Login")
        .WithOpenApi();
    }
    private void ConfigureChaosMonkeyService(WebApplication app)
    {
        // login
        app.MapGet("/break/login", () =>
        {
            Log.Information("/break/login");
            Globals.BreakLogin.Set(true);
            return Results.StatusCode(200);
        })
        .WithName("break/login")
        .WithOpenApi();

        app.MapGet("/fix/login", () =>
        {
            Log.Information("/fix/login");
            Globals.BreakLogin.Set(false);
            return Results.StatusCode(200);
        })
        .WithName("fix/login")
        .WithOpenApi();

        app.MapGet("/status/login", () =>
        {
            Log.Information("/status/login");
            bool value = Globals.BreakLogin.Get();
            if (value)
            {
                return Results.Ok("Broken");
            }
            else
            {
                return Results.Ok("Operational");
            }
        })
        .WithName("status/login")
        .WithOpenApi();
    }
}
