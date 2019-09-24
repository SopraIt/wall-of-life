using Newtonsoft.Json;

public class Prize
{
    private string imgPath
    {
        get
        {
            return "prizes\\";
        }
    }

    [JsonIgnore]
    public string prizeImg
    {

        get
        {
            return imgPath + prizeType;
        }
    }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("type")]
    public string prizeType { get; set; }

    [JsonProperty("description")]
    public string description { get; set; }

    [JsonProperty("source")]
    public string source { get; set; }
}