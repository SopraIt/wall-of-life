using Newtonsoft.Json;
using System.Collections.Generic;

public class Classes
{
    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("items")]
    public List<Item> items { get; set; }
}