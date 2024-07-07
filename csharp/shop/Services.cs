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
        // products
        app.MapGet("/products", () =>
        {
            Log.Information("/products");

            int flagid = Globals.GetFlagId("products");
            AtomicBoolean flag = Globals.BreakFlags[flagid];

            var result = flag.Get() ? null : Consts.GetProducts();
            return result;
        })
        .WithName("Products")
        .WithOpenApi();

        // checkout
        app.MapPost("/checkout", async (CheckoutRequest req) =>
        {
            Log.Information("/checkout by {0}", req.username);

            int flagid = Globals.GetFlagId("checkout");
            AtomicBoolean flag = Globals.BreakFlags[flagid];

            if (flag.Get() == false)
            {
                foreach (var item in req.cartItems)
                {
                    if (item.Value > 0)
                    {
                        Log.Information("{0} bought productid:{1}, product:{2}, quantity:{3}", req.username, item.Key, Consts.GetProductName(item.Key), item.Value);
                    }
                };
                // TODO: ship the products
                return await Task.FromResult(Results.Json(new RestResult("Success", "Thank you for purchasing!", "")));
            }
            else
            {
                return await Task.FromResult(Results.Json(new RestResult("Failure", "Checkout service is broken", "")));
            }
        })
       .WithName("Checkout")
       .WithOpenApi();

        // login
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

        // service status
        app.MapGet("/status", () =>
        {
            Log.Information("/status");

            // var result = new bool[] { true, true, true, true };
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (string feature in Globals.Flags)
            {
                int id = Globals.GetFlagId(feature);
                result[feature] = Globals.BreakFlags[id].Get() ? false : true;
            }

            return Results.Json(result);
        })
        .WithName("status")
        .WithOpenApi();
    }
    private void ConfigureChaosMonkeyService(WebApplication app)
    {
        foreach (string feature in Globals.Flags)
        {
            ConfigureBreakHandlers(feature, app);
        }
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
