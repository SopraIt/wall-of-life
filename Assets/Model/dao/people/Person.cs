using System.Collections.Generic;
using Newtonsoft.Json;

public class Person
{
    private string imgPath {
        get {
            return "login-profile/";
        }
    }

    [JsonIgnore]
    public string profileImg
    {
        get
        {
            return imgPath + avatar.name;
        }
    }
    [JsonIgnore]
    public string profileImgUrl
    {
        get
        {
            return avatar.url;
        }
    }

    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("firstName")]
    public string personName { get; set; }

    [JsonProperty("lastName")]
    public string surname { get; set; }

    [JsonProperty("dateOfBirth")]
    public string dateOfBirth { get; set; }

    [JsonProperty("age")]
    public string age { get; set; }

    [JsonProperty("level")]
    public string level { get; set; }

    [JsonProperty("image")]
    public Avatar avatar { get; set; }

    [JsonProperty("games")]
    public List<Game> games { get; set; }

    [JsonProperty("hand")]
    public string hand { get; set; }

    [JsonProperty("skipTutorial")]
    public string skipTutorial { get; set; }

    [JsonIgnore]
    public string getHand
    {
        get
        {
            if ("r".Equals(hand)) {
                return "HandRight";
            }
            if ("l".Equals(hand))
            {
                return "HandLeft";
            }
            return "HandRight";
        }
    }
}