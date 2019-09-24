using Newtonsoft.Json;

public class Item
{
    [JsonProperty("itemid")]
    public string id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("text")]
    public string text { get; set; }

    [JsonProperty("shorttext")]
    public string shorttext { get; set; }

    [JsonProperty("type")]
    public string type { get; set; }

    [JsonProperty("price")]
    public string Price { get; set; }

    [JsonProperty("category")]
    public string category { get; set; }

    [JsonIgnore]
    public bool selectable { get; set; }

    [JsonIgnore]
    public bool draggable { get; set; }

    [JsonIgnore]
    public bool container { get; set; }
    
    private string itemPath
    {
        get
        {
            return "items\\";
        }
    }
    
    private string charactersPath
    {
        get
        {
            return "characters\\";
        }
    }

    [JsonIgnore]
    public string itemImage {
        get {
            return ("person".Equals(type) ? charactersPath : itemPath) + type + "_" + name;
        }
    }

}