using Serilog;
using System.Text.Json;
using System.Text;
using System.ComponentModel;

class Services
{
    private Random rand = new Random();

    public void Run(WebApplication app)
    {
        ConfigureShopService(app);
        ConfigureChaosMonkeyService(app);
        app.Run();
    }

    private IResult Reset()
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
    }

    private IResult Login(string username)
    {
        Log.Information($"/login {username}");

        int flagid = Globals.GetFlagId("login");
        AtomicInteger flag = Globals.BreakFlags[flagid];

        if (flag.Get() == 2)
        {
            string reason = Reason([
                new ReasonPercent(50, "Network Issue"),
                new ReasonPercent(50, "Hardware Failures"),
            ]);
            int sleepMs = rand.Next(1000, 5000);
            Log.Warning($"user: {username} login service {sleepMs} ms delayed due to '{reason}'");
            Thread.Sleep(sleepMs);
        }

        if (flag.Get() == 1)
        {
            ReasonStatusCode rs = FailureReason([
                new ReasonStatusCode(30, StatusCodes.Status400BadRequest, "Bad login request"),
                new ReasonStatusCode(30, StatusCodes.Status401Unauthorized, "Unauthorized"),
                new ReasonStatusCode(30, StatusCodes.Status403Forbidden, "Forbidden"),
            ]);
            Log.Error($"user: {username} login failed, reason: {rs.Reason}");
            return Results.Json(new RestResult("Failure", rs.Reason, ""), statusCode: rs.StatusCode);
        }
        return Results.Json(new RestResult("Success", "", ""));
    }

    private IResult Products()
    {
        Log.Information("/products");

        int flagid = Globals.GetFlagId("products");
        AtomicInteger flag = Globals.BreakFlags[flagid];

        if (flag.Get() == 1)
        {
            ReasonStatusCode rs = FailureReason([
                new ReasonStatusCode(95, StatusCodes.Status500InternalServerError, "Product server is down"),
            ]);
            Log.Error($"/products failed. {rs.Reason}");
            return Results.Json(new Product[] { }, statusCode: rs.StatusCode);
        }
        else if (flag.Get() == 2)
        {
            string reason = Reason([
                new ReasonPercent(50, "Network Issue"),
                new ReasonPercent(50, "Hardware Failures"),
            ]);
            int sleepMs = rand.Next(1000, 5000);
            Log.Warning($"product service is {sleepMs} ms delayed due to {reason}");
            Thread.Sleep(sleepMs);
        }
        return Results.Json(Consts.GetProducts());
    }

    private async Task<IResult> CheckOut(CheckoutRequest req)
    {
        Log.Information($"/checkout by {req.username}");

        int flagid = Globals.GetFlagId("checkout");
        AtomicInteger flag = Globals.BreakFlags[flagid];

        if (flag.Get() == 0 || flag.Get() == 2)
        {
            if (flag.Get() == 2)
            {
                string reason = Reason([
                    new ReasonPercent(50, "Too many requests"),
                    new ReasonPercent(50, "Credit card network slowness"),
                ]);
                int sleepMs = rand.Next(1000, 5000);
                Log.Warning($"user: {req.username} checkout service {sleepMs} ms delayed due to {reason}");
                Thread.Sleep(sleepMs);
            }
            foreach (var item in req.cartItems)
            {
                if (item.Value > 0)
                {
                    Log.Information($"user: {req.username} bought productid:{item.Key}, product:{Consts.GetProductName(item.Key)}, quantity:{item.Value}");
                }
            };
            // ship the products
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = string.Format($"http://localhost:{Consts.WEBAPI_PORT}/delivery");

                    // Make the GET request
                    string jsonData = JsonSerializer.Serialize(req);
                    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    Log.Information($"user: {req.username} Calling {url} with {jsonData}");
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
                        Log.Error($"user: {req.username} Unexpected response from delivery service");
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
                    Log.Error($"user: {req.username} Failed in delivery. Err: {e.Message}");
                    return await Task.FromResult(Results.Json(new RestResult("Failure", "Failed in delivery", e.Message)));
                }
            }
        }
        else
        {
            ReasonStatusCode rs = FailureReason([
                new ReasonStatusCode(50, StatusCodes.Status401Unauthorized, "Credit card not authorized"),
                new ReasonStatusCode(50, StatusCodes.Status500InternalServerError, "Checkout server is down"),
            ]);
            Log.Error($"user: {req.username} /checkout failed. {rs.Reason}");
            return await Task.FromResult(Results.Json(new RestResult("Failure", rs.Reason, ""), statusCode: rs.StatusCode));
        }
    }

    private async Task<IResult> Delivery(CheckoutRequest req)
    {
        Log.Information($"/delivery by {req.username}");

        int flagid = Globals.GetFlagId("delivery");
        AtomicInteger flag = Globals.BreakFlags[flagid];

        if (flag.Get() == 0 || flag.Get() == 2)
        {
            if (flag.Get() == 2)
            {
                string reason = Reason([
                    new ReasonPercent(20, "Lack of Drivers"),
                    new ReasonPercent(40, "Traffic Jam"),
                    new ReasonPercent(20, "Bad Weather"),
                    new ReasonPercent(20, "Traffic Accident"),
                ]);
                int sleepMs = rand.Next(1000, 5000);
                Log.Warning($"user: {req.username} delivery {sleepMs} ms delayed due to {reason}");
                Thread.Sleep(sleepMs);
            }

            foreach (var item in req.cartItems)
            {
                if (item.Value > 0)
                {
                    Log.Information($"user: {req.username} requested delivery for productid:{item.Key}, product:{Consts.GetProductName(item.Key)}, quantity:{item.Value}");
                }
            };
            return await Task.FromResult(Results.Json(new RestResult("Success", "Delivery successful", "")));
        }
        else
        {
            ReasonStatusCode rs = FailureReason([
                new ReasonStatusCode(50, StatusCodes.Status500InternalServerError, "Traffic Accident"),
                new ReasonStatusCode(50, StatusCodes.Status500InternalServerError, "Delivery server is down"),
            ]);
            Log.Error($"user: {req.username} /delivery failed. {rs.Reason}");
            return await Task.FromResult(Results.Json(new RestResult("Failure", rs.Reason, ""), statusCode: rs.StatusCode));
        }
    }

    private IResult Status()
    {
        Log.Information("/status");

        Dictionary<string, int> result = new Dictionary<string, int>();
        foreach (string feature in Globals.Flags)
        {
            int id = Globals.GetFlagId(feature);
            result[feature] = Globals.BreakFlags[id].Get();
        }

        return Results.Json(result);
    }

    private void ConfigureShopService(WebApplication app)
    {
        // reset
        app.MapGet("/reset", Reset)
        .WithName("Reset")
        .WithOpenApi();

        // products
        app.MapGet("/products", Products)
        .WithName("Products")
        .WithOpenApi();

        // checkout
        app.MapPost("/checkout", CheckOut)
        .WithName("Checkout")
        .WithOpenApi();

        // login
        app.MapGet("/login", Login)
        .WithName("Login")
        .WithOpenApi();

        // delivery
        app.MapPost("/delivery", Delivery)
        .WithName("Delivery")
        .WithOpenApi();

        // service status
        app.MapGet("/status", Status)
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
        int flagid = Globals.GetFlagId(feature);

        app.MapGet("/fix/" + flagid, () =>
        {
            AtomicInteger flag = Globals.BreakFlags[flagid];

            Log.Information("/fix/" + flagid);
            flag.Set(0);
            return Results.StatusCode(200);
        })
        .WithName("fix/" + flagid)
        .WithOpenApi();

        app.MapGet("/break/" + flagid, () =>
        {
            AtomicInteger flag = Globals.BreakFlags[flagid];

            Log.Information("/break/" + flagid);
            flag.Set(1);
            return Results.StatusCode(200);
        })
        .WithName("break/" + flagid)
        .WithOpenApi();

        app.MapGet("/slow/" + flagid, () =>
        {
            AtomicInteger flag = Globals.BreakFlags[flagid];

            Log.Information("/slow/" + flagid);
            flag.Set(2);
            return Results.StatusCode(200);
        })
        .WithName("slow/" + flagid)
        .WithOpenApi();

        app.MapGet("/status/" + flagid, () =>
        {
            AtomicInteger flag = Globals.BreakFlags[flagid];

            Log.Information("/status/" + flagid);
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
        .WithName("status/" + flagid)
        .WithOpenApi();
    }


    class ReasonPercent
    {
        public int Percent;
        public string Reason;

        public ReasonPercent(int percent, string reason)
        {
            Percent = percent;
            Reason = reason;
        }
    }

    class ReasonStatusCode
    {
        public int Percent;
        public int StatusCode;
        public string Reason;

        public ReasonStatusCode(int percent, int statuscode, string reason)
        {
            Percent = percent;
            StatusCode = statuscode;
            Reason = reason;
        }
    }

    private string Reason(ReasonPercent[] reasons)
    {
        int r = rand.Next(100); // r = [0, 100)

        foreach (ReasonPercent rp in reasons)
        {
            if (r < rp.Percent) return rp.Reason;
            r -= rp.Percent;
        }
        return "Unknown Reason";
    }
    private ReasonStatusCode FailureReason(ReasonStatusCode[] reasons)
    {
        int r = rand.Next(100); // r = [0, 100)

        foreach (ReasonStatusCode rs in reasons)
        {
            if (r < rs.Percent) return rs;
            r -= rs.Percent;
        }
        return new ReasonStatusCode(0, StatusCodes.Status500InternalServerError, "Internal Server Error");
    }
}
