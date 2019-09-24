using Newtonsoft.Json;

public class Avatar
{
    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("url")]
    public string url { get; set; }

}