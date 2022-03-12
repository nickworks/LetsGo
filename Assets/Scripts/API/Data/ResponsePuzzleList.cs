using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResponsePuzzleList
{
    [System.Serializable]
    public class Puzzle {
        public int id;
        public string name;
        public string created; // date
        public string modified; // date
    }
    public int count;
    public string next;
    public string prev;
    public Puzzle[] results;
}
