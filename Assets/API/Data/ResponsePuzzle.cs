using System;
using Newtonsoft.Json;

[Serializable]
public class ResponsePuzzle
{
    public int id;
    public string name;
    public string type;
    public ResponsePuzzleCollection.PuzzleCollection collection;
    public string created;
    public string modified;
    public bool has_solution;
    public int width;
    public int height;
    public float order;
    public ResponsePuzzleCollection.PuzzleCollection.Owner owner;
    [JsonProperty("private")]
    public bool is_private;
    public float rating;
    public int rank;
    public int rank_count;
    public int solved_count;
    public int attempt_count;
    public int view_count;
    public Puzzle puzzle;
    [Serializable]
    public class Puzzle {
        public string name;
        public string mode;
        public int width;
        public int height;
        public string puzzle_type;
        public int puzzle_rank;
        public string initial_player;
        public string puzzle_player_move_mode;
        public string puzzle_opponent_move_mode;
        public string puzzle_description;
        public string puzzle_collection;
        public ResponsePuzzleCollection.PuzzleCollection.Puzzle.BoardState initial_state;
        public MoveTree move_tree;
    }
    [Serializable]
    public class MoveTree {
        public int x;
        public int y;
        public string text;
        public bool correct_answer;
        public bool wrong_answer;
        public MoveTree[] branches;
    }
}