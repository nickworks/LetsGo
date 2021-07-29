using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResponseGameList
{
    public int count = 0;
    public string next = "";
    public string prev = "";
    public GameOGS[] results = new GameOGS[0];
}
[System.Serializable]
public class GameOGS {
    public int id = 0;
    public Players players;
    public string name = "";
    public int creator = 0;
    public string mode = "";
    public string source = "";
    public int black = 0;
    public int white = 0;
    public int width = 19;
    public int height = 19;
    public bool ranked = false;
    public int handicap = 0;
    public string komi = "";
    public string outcome = "";
    public bool black_lost = false;
    public bool white_lost = true;
    public bool annulled = false;
    public string started = "";
    public string ended = "";
}
[System.Serializable]
public class Players {
    public Friend white;
    public Friend black;
}
