using Newtonsoft.Json;

public class LastId
{
    [JsonProperty("item")]
    public string item { get; set; }

    [JsonProperty("category")]
    public string category { get; set; }

    [JsonProperty("prize")]
    public string prize { get; set; }

    [JsonProperty("person")]
    public string person { get; set; }

    [JsonProperty("match")]
    public string match { get; set; }
}