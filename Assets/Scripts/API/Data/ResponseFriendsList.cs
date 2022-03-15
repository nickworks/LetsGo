using System;

[Serializable]
public class ResponseFriendsList {
    public int count = 0;
    public string next = "";
    public string prev = "";
    public Friend[] results = new Friend[0];
}
[Serializable]
public class Friend {
    [Serializable]
    public class Rating {
        public float version = 0;
        public RatingStats overall;
    }
    public int id = 0;
    public string username = "";
    public string country = "";
    public string icon = "";
    public float ranking = 0;
    public bool professional = false;
    public string ui_class = "";
    public Rating ratings;
}
[Serializable]
public struct RatingStats {
    public float rating;
    public float deviation;
    public float volatility;
}
