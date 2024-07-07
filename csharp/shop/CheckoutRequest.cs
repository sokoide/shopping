using System.Text.Json.Serialization;
public class CheckoutRequest
{
    [JsonPropertyName("cartItems")]
    public Dictionary<string, int> cartItems { get; set; }

    [JsonPropertyName("username")]
    public string username { get; set; }

    public CheckoutRequest()
    {
        cartItems = new Dictionary<string, int>();
        username = "";
    }
}
