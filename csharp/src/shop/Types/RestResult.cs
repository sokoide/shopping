using System.Text.Json.Serialization;

public class RestResult
{
    [JsonPropertyName("result")]
    public string Result { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("error")]
    public string Error { get; set; }
    public RestResult(string result, string message, string error)
    {
        Result = result;
        Message = message;
        Error = error;
    }
}
