using Newtonsoft.Json;
using System.Collections.Generic;

public class Question
{
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("text")]
    public string text { get; set; }
}