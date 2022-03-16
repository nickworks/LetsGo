using System;
using Newtonsoft.Json;

[Serializable]
public class ResponsePuzzleCollection {
    public int count;
    public string next;
    public string previous;
    public PuzzleCollection[] results;
    [Serializable]
    public class PuzzleCollection {
        public class Owner {
            public int id;
            public string username;
            public string country;
            public string icon;
            public Friend.Rating ratings;
            public float ranking;
            public bool professional;
            public string ui_class;
        }
        public class Puzzle {
            public class BoardState {
                public string white;
                public string black;
            }
            public int id;
            public BoardState initial_state;
            public int width;
            public int height;
        }
        public Owner owner;
        public string name;
        public string created;
        [JsonProperty("private")]
        public bool is_private;
        public float price;
        public Puzzle starting_puzzle;
        public float rating;
        public int rating_count;
        public int puzzle_count;
        public int min_rank;
        public int max_rank;
        public int view_count;
        public int solved_count;
        public int attempt_count;
        public bool color_transform_enabled;
        public bool position_transform_enabled;
    }
}

