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
    public GameDataOGS gamedata = null;
}

[System.Serializable]
public class Players {
    public Friend white;
    public Friend black;
}
[System.Serializable]
public class GameDataOGS {
    public bool score_stones = false;
    public float time_canadian_moves = 0;
    public bool allow_ko = false;
    public bool allow_superko = false;
    public int height = 19;
    public int width = 19;
    public string time_control = "";
    public float time_fischer_max = 0;
    public bool score_passes = true;
    public bool free_handicap_placement = false;
    public int[,] moves = null;
    public int black_player_id = 0;
    public int white_player_id = 0;
    public int winner = 0;
    public float handicap = 0;
    public bool score_prisoners = true;
    public bool score_territory = true;
    public bool allow_self_capture = false;
    public string phase = "";
    public string outcome = "";
    public string initial_player = "black";
    public bool pause_on_weekend = false;
}
