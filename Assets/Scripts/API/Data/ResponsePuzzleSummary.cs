using System;
using Newtonsoft.Json;

[Serializable]
public class ResponsePuzzleSummary
{
    [Serializable]
    public class Puzzle {
        public int id;
        public string name;
    }
}