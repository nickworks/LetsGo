using System;
using Newtonsoft.Json;

[Serializable]
public class ResponseMyProfile
{
    [Serializable]
    public class Rating {
        public float version = 0;
        public RatingStats overall;
        public RatingStats live;
        public RatingStats blitz;
        public RatingStats correspondence;
        [JsonProperty("9x9")]
        public RatingStats overall_9x9;
        [JsonProperty("13x13")]
        public RatingStats overall_13x13;
        [JsonProperty("19x19")]
        public RatingStats overall_19x19;
        [JsonProperty("live-9x9")]
        public RatingStats live_9x9;
        [JsonProperty("live-13x13")]
        public RatingStats live_13x13;
        [JsonProperty("live-19x19")]
        public RatingStats live_19x19;
        [JsonProperty("blitz-9x9")]
        public RatingStats blitz_9x9;
        [JsonProperty("blitz-13x13")]
        public RatingStats blitz_13x13;
        [JsonProperty("blitz-19x19")]
        public RatingStats blitz_19x19;
        [JsonProperty("correspondence-9x9")]
        public RatingStats correspondence_9x9;
        [JsonProperty("correspondence-13x13")]
        public RatingStats correspondence_13x13;
        [JsonProperty("correspondence-19x19")]
        public RatingStats correspondence_19x19;
    }

    public int id = 0;
    public string username = "";
    public Rating ratings;
    public float ranking = 0;
    public string about = "";

    //HATEOAS
    public string settings = "";
    public string account_settings = "";
    public string friends = "";
    public string games = "";
    public string challenges = "";
    public string groups = "";
    public string mail = "";
    public string tournaments = "";
    public string notifications = "";
}
