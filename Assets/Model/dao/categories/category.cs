using Newtonsoft.Json;

public class Category {

    [JsonProperty("categoryid")]
    public string id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("selectable")]
    public string selectable { get; set; }

    [JsonProperty("draggable")]
    public string draggable { get; set; }

    [JsonProperty("container")]
    public string container { get; set; }
}