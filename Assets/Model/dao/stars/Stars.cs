using Newtonsoft.Json;

public class Stars
{
    [JsonProperty("1")]
    public string First { get; set; }

    [JsonProperty("2")]
    public string Second { get; set; }

    [JsonProperty("3")]
    public string Third { get; set; }
}