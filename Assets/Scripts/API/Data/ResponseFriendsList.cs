using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResponseFriendsList {
    public int count = 0;
    public string next = "";
    public string prev = "";
    public Friend[] results = new Friend[0];
}
[System.Serializable]
public class Friend {
    public int id = 0;
    public string username = "";
    public string country = "";
    public string icon = "";
    public float ranking = 0;
    public bool professional = false;
    public string ui_class = "";
    public Rating ratings;
}
[System.Serializable]
public class Rating {
    public float version = 0;
    public RatingStats overall;
}
[System.Serializable]
public struct RatingStats {
    public float rating;
    public float deviation;
    public float volatility;
}
