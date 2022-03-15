using System;
using Newtonsoft.Json;
[Serializable]
public class ResponseActiveBots {
    public int id;
    public string username;
    [JsonProperty("icon-url")]
    public string icon_url;
    public string ui_class;
    public ResponseMyProfile.Rating ratings;
    public float ranking;
    public float rating;
    public float pro;
}