using Newtonsoft.Json;
using System.Collections.Generic;

public class DatabaseData
{
    [JsonProperty("people")]
    public List<Person> people  { get; set;}

    [JsonProperty("categories")]
    public List<Category> categories { get; set; }

    [JsonProperty("prizes")]
    public List<Prize> prizes { get; set; }

    [JsonProperty("classes")]
    public List<Classes> classes { get; set; }

    [JsonProperty("answers")]
    public Answer answers { get; set; }

    [JsonProperty("lastId")]
    public LastId lastId { get; set; }

    [JsonProperty("stars")]
    public Stars Stars { get; set; }
}