using Newtonsoft.Json;
using System.Collections.Generic;

public class Steps
{
    [JsonProperty("action")]
    public List<Question> questions { get; set; }

    [JsonProperty("type")]
    public string type { get; set; }
}