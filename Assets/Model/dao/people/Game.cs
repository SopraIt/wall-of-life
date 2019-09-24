using Newtonsoft.Json;

public class Game
{
    [JsonProperty("matchid")]
    public string id { get; set; }

    [JsonProperty("date")]
    public string date { get; set; }

    [JsonProperty("level")]
    public string level { get; set; }

    [JsonProperty("correctanswers")]
    public string correctAnswers { get; set; }

    [JsonProperty("totalhelp")]
    public string totalHelp { get; set; }

    [JsonProperty("totaltime")]
    public string totalTime { get; set; }

}