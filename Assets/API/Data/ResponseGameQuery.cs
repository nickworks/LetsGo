using System;
using Newtonsoft.Json;

[Serializable]
public class ResponseGameQuery {
    // these are the parameters for the query:
    public string list;
    public string by;
    public int from;
    public int limit;
    // these are the results:
    public int size; // number of live games
    
    public GameOGS[] results = new GameOGS[0]; // games returned
    [Serializable]
    public class GameOGS {
        public int id;
        public int[] group_ids = new int[0];
        public string phase;
        public string name;
        public int player_to_move;
        public int width;
        public int height;
        public int move_number;
        public int paused;

        [JsonProperty("private")]
        public bool is_private;

        public Player black;
        public Player white;

        public float time_per_move;
        public bool rengo;
        public object rengo_teams = null; // ??
        public bool rengo_casual_mode;
        public int dropped_player;
        public int[] _participants;
        public bool ranked;
        public float handicap;
        public float komi;
        public bool bot_game;
        public bool in_beginning;
        public bool in_middle;
        public bool in_end;
        public object group_ids_map = null; // ??
    }
    [Serializable]
    public class Player {
        public string username;
        public int id;
        public float rank;
        public bool professional;
        public bool accepted;
        public Friend.Rating ratings;
    }
}