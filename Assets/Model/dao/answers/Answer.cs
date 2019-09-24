using Newtonsoft.Json;
using System.Collections.Generic;

public class Answer
{
    [JsonProperty("briefing")]
    public List<Steps> briefing { get; set; }

    [JsonProperty("marketstand")]
    public List<Steps> marketstand { get; set; }
}