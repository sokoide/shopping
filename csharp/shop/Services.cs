using Serilog;
using System.Text.Json;
using System.Text;
using Grpc.Core;

class Services
{
    private Random rand = new Random();

    public void Run(WebApplication app)
    {
        ConfigureShopService(app);
        ConfigureChaosMonkeyService(app);
        app.Run();
    }

    private void ConfigureShopService(WebApplication app)
    {
        // reset
        app.MapGet("/reset", () =>
        {
            Log.Information("/reset");
            Globals.Reset();

            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (string feature in Globals.Flags)
            {
                int id = Globals.GetFlagId(feature);
                result[feature] = Globals.BreakFlags[id].Get();
            }

            return Results.Json(result);
        })
        .WithName("Reset")
        .WithOpenApi();

        // products
        app.MapGet("/products", () =>
        {
            Log.Information("/products");

            int flagid = Globals.GetFlagId("products");
            AtomicInteger flag = Globals.BreakFlags[flagid];

            if (flag.Get() == 1)
            {
                Log.Error("product service is down");
                int st = StatusCodes.Status500InternalServerError;
                return Results.Json(new Product[] { }, statusCode: st);
            }
            return Results.Json(Consts.GetProducts());
        })
        .WithName("Products")
        .WithOpenApi();

        // checkout
        app.MapPost("/checkout", async (CheckoutRequest req) =>
        {
            Log.Information("/checkout by {0}", req.username);

            int flagid = Globals.GetFlagId("checkout");
            AtomicInteger flag = Globals.BreakFlags[flagid];

            if (flag.Get() == 0)
            {
                foreach (var item in req.cartItems)
                {
                    if (item.Value > 0)
                    {
                        Log.Information("{0} bought productid:{1}, product:{2}, quantity:{3}", req.username, item.Key, Consts.GetProductName(item.Key), item.Value);
                    }
                };
                // ship the products
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string url = string.Format("http://localhost:{0}/delivery", Consts.WEBAPI_PORT);

                        // Make the GET request
                        string jsonData = JsonSerializer.Serialize(req);
                        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        Log.Information("Calling {0} with {1}", url, jsonData);
                        HttpResponseMessage response = await client.PostAsync(url, content);

                        // Ensure the request was successful
                        response.EnsureSuccessStatusCode();

                        // Read the response content
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Process the response (e.g., print it to the console)
                        Console.WriteLine(responseBody);

                        RestResult? res = JsonSerializer.Deserialize<RestResult>(responseBody);
                        if (res == null)
                        {
                            Log.Error("Unexpected response from delivery service");
                            return await Task.FromResult(Results.Json(new RestResult("Failure", "Failed in delivery", "res is null")));
                        }
                        else if (res.Result != "Success")
                        {
                            return await Task.FromResult(Results.Json(res));
                        }
                        return await Task.FromResult(Results.Json(new RestResult("Success", "Thank you for purchasing!", "")));
                    }
                    catch (HttpRequestException e)
                    {
                        Log.Error("Failed in delivery. Err: {Message}", e.Message);
                        return await Task.FromResult(Results.Json(new RestResult("Failure", "Failed in delivery", e.Message)));
                    }
                }
            }
            else
            {
                Log.Error("checkout service is down");
                int st = StatusCodes.Status500InternalServerError;
                return await Task.FromResult(Results.Json(new RestResult("Failure", "Checkout service is down", ""), statusCode: st));
            }
        })
        .WithName("Checkout")
        .WithOpenApi();

        // login
        app.MapGet("/login", (string username) =>
        {
            Log.Information("/login {0}", username);

            int flagid = Globals.GetFlagId("login");
            AtomicInteger flag = Globals.BreakFlags[flagid];

            if (flag.Get() == 1)
            {
                int r = rand.Next(100); // r = [0, 100)
                int st;
                string msg;

                switch (r)
                {
                case int n when (0 <= n && n < 30):
                    st = StatusCodes.Status400BadRequest;
                    msg = "Bad login request";
                    break;
                case int n when (30 <= n && n < 60):
                    st = StatusCodes.Status401Unauthorized;
                    msg = "Unauthorized";
                    break;
                case int n when (60 <= n && n < 90):
                    st = StatusCodes.Status403Forbidden;
                    msg = "Forbidden";
                    break;
                default:
                    st = StatusCodes.Status500InternalServerError;
                    msg = "Internal server error";
                    break;
                }
                Log.Error($"{username} login failed, reason: {msg}");
                return Results.Json(new RestResult("Failure", msg, ""), statusCode: st);
            }
            return Results.Json(new RestResult("Success", "", ""));
        })
        .WithName("Login")
        .WithOpenApi();

        // delivery
        app.MapPost("/delivery", async (CheckoutRequest req) =>
        {
            Log.Information("/delivery by {0}", req.username);

            int flagid = Globals.GetFlagId("delivery");
            AtomicInteger flag = Globals.BreakFlags[flagid];

            if (flag.Get() == 0)
            {
                foreach (var item in req.cartItems)
                {
                    if (item.Value > 0)
                    {
                        Log.Information("{0} requested delivery for productid:{1}, product:{2}, quantity:{3}", req.username, item.Key, Consts.GetProductName(item.Key), item.Value);
                    }
                };
                return await Task.FromResult(Results.Json(new RestResult("Success", "Delivery successful", "")));
            }
            else
            {
                Log.Error("delivery service is down");
                int st = StatusCodes.Status500InternalServerError;
                return await Task.FromResult(Results.Json(new RestResult("Failure", "Delivery service is down", ""), statusCode: st));
            }
        })
        .WithName("Delivery")
        .WithOpenApi();

        // service status
        app.MapGet("/status", () =>
        {
            Log.Information("/status");

            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (string feature in Globals.Flags)
            {
                int id = Globals.GetFlagId(feature);
                result[feature] = Globals.BreakFlags[id].Get();
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
            AtomicInteger flag = Globals.BreakFlags[flagid];

            Log.Information("/break/" + feature);
            flag.Set(1);
            return Results.StatusCode(200);
        })
        .WithName("break/" + feature)
        .WithOpenApi();

        app.MapGet("/fix/" + feature, () =>
        {
            int flagid = Globals.GetFlagId(feature);
            AtomicInteger flag = Globals.BreakFlags[flagid];

            Log.Information("/fix/" + feature);
            flag.Set(0);
            return Results.StatusCode(200);
        })
        .WithName("fix/" + feature)
        .WithOpenApi();

        app.MapGet("/status/" + feature, () =>
        {
            int flagid = Globals.GetFlagId(feature);
            AtomicInteger flag = Globals.BreakFlags[flagid];

            Log.Information("/status/" + feature);
            int value = flag.Get();
            if (value == 0)
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
