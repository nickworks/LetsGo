using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Newtonsoft.Json;

public class PlayController : MonoBehaviour {
    public int sizex = 9;
    public int sizey = 9;
    public int sizez = 1;

    static public PlayController singleton {get; private set;}

    Stone[,,] data;

    List<Move> history = new List<Move>();

    private ResponsePuzzle.MoveTree _move_tree;
    private ResponsePuzzle.MoveTree move_tree {
        get { return _move_tree; }
        set {
            _move_tree = value;
            moveTree = JsonConvert.SerializeObject(_move_tree);
        }
    }
    [TextArea(1,5)]
    public string moveTree;

    public byte currentPlayerTurn { get; private set; }
    public byte localPlayer { get; private set; }
    public DummyController dummyObject;

    public GobanRenderer gobanPrefab;

    public byte numberOfPlayers = 2;

    //public bool isPlayer1Turn { get; private set; }
    public int stonesPerPlay = 1;
    public int stonesRemaining = 0;
    public GobanRenderer goban { get; private set; }

    // Use this for initialization
    void Start () {

        if(singleton != null){
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }
    void OnDestroy(){
        if(singleton == this) singleton = null;
    }
    private void MakeFreshGoban(int x, int y, int z){

        history.Clear();

        MakeData(x,y,z); // make data

        if (goban) Destroy(goban.gameObject);
        goban = Instantiate(gobanPrefab);
        goban.Display(data); // make visuals
    }
    // this begins a new game at whatever dimensions are stored in this MonoBehaviour (temporary?)
    public void BeginGame(){
        BeginGame(sizex, sizey, sizez);
    }
    // this begins a new local game
    public void BeginGame(int x, int y, int z)
    {
        MakeFreshGoban(x, y, z);
        NextTurn(); // begin play
    }
    // this deserializes a JSON string
    // into a ResponsePuzzle, and then
    // begins playing that puzzle
    public void BeginPuzzle(string json){
        BeginPuzzle(JsonConvert.DeserializeObject<ResponsePuzzle>(json));
    }
    // this loads a puzzle from OGS
    public void BeginPuzzle(int id){
        
        RestOGS.Get_Puzzle(id, BeginPuzzle); // see below
    }
    // this creates a new goban and
    // begins a new puzzle play session
    public void BeginPuzzle(ResponsePuzzle puzzle){

        if(puzzle == null) return;

        numberOfPlayers = 2;
        stonesPerPlay = 1;

        Debug.Log($"The puzzle -- {puzzle.name} -- was created by {puzzle.owner.username}");

        MakeFreshGoban(puzzle.puzzle.width, puzzle.puzzle.height, 1);

        //puzzle.name
        //puzzle.collection.name
        //puzzle.collection.puzzle_count
        //puzzle.puzzle.puzzle_description
        //puzzle.puzzle.puzzle_player_move_mode
        
        move_tree = puzzle.puzzle.move_tree;

        switch(puzzle.puzzle.initial_player){
            case "black": localPlayer = 2; break;
            case "white": localPlayer = 1; break;
        }
        currentPlayerTurn = localPlayer;
        ApplySGF(puzzle.puzzle.initial_state.white, 2);
        ApplySGF(puzzle.puzzle.initial_state.black, 1);
        goban.Display(data);

        PanelSwitcher.singleton.ShowHUD();
            //RestOGS.API.Get_PuzzleCollectionSummary(id);
            //RestOGS.API.Get_PuzzleRate(id);
    }
    public Bounds GetBounds()
    {
        if (data == null) return new Bounds();
        int x = data.GetLength(0) - 1;
        int y = data.GetLength(2) - 1;
        int z = data.GetLength(1) - 1;
        Vector3 size = new Vector3(x, y, z);
        return new Bounds(size / 2, size);
    }
    public Vector3 GetCenter() {
        if (data == null) return Vector3.zero;
        return new Vector3(data.GetLength(0)-1, data.GetLength(2)-1, data.GetLength(1)-1) / 2;
    }
    public int SizeZ()
    {
        if(data == null) return 0;
        return data.GetLength(2);
    }
    /// <summary>
    /// Fills the board with stones, and sets every stone's value to 0.
    /// </summary>
    void MakeData(int X, int Y, int Z)
    {
        data = new Stone[X, Y, Z];
        for (byte x = 0; x < data.GetLength(0); x++)
        {
            for (byte y = 0; y < data.GetLength(1); y++)
            {
                for (byte z = 0; z < data.GetLength(2); z++)
                {
                    data[x, y, z] = new Stone(x, y, z);
                }
            }
        }
    }
    
    /// <summary>
    /// The current player makes a play at the designated board position. If the play is legal,
    /// immediately resolve captures, remove pieces, and switch turns.
    /// </summary>
    /// <param name="x">Board x position.</param>
    /// <param name="y">Board y position.</param>
    /// <returns>Whether or not the move was legal. If false, it is still the same players turn.</returns>
    public bool PlayStoneAt(int x, int y, int z = 0)
    {
        //print("ATTEMPT PLAY AT: " + x + " " + y + " " + z);
        if (!IsVacantSpot(x, y, z)) return false;

        byte[,,] boardState = BoardAsBytes();

        SetStoneValue(x, y, z, currentPlayerTurn);

        if (CaptureStones()) // attempt to capture stones
        {
            stonesRemaining--;
            RecordMove(x, y, z, boardState);

            if (stonesRemaining <= 0) NextTurn(); // trigger next player
            return true; // successful play
        }
        SetStoneValue(x, y, z, 0); // undo move
        return false; // illegal move, no change
    }

    private void RecordMove(int x, int y, int z, byte[,,] priorState)
    {

        history.Add(new Move(x, y, z, move_tree, priorState)); // add move to history
        
        // advance the move_tree:

        if(move_tree == null) return;
        if(move_tree.branches == null) return;
        if(move_tree.branches.Length == 0) return;
        bool foundIt = false;
        foreach(ResponsePuzzle.MoveTree tree in move_tree.branches){
            if(tree.x == x && tree.y == y){
                move_tree = tree;
                if(move_tree.wrong_answer) Debug.Log("Incorrect");
                if(move_tree.correct_answer) Debug.Log("Correct!");
                foundIt = true;
                break;
            }
        }
        if(!foundIt) {
            move_tree = null;
            Debug.Log("Incorrect");
        }
    }
    public byte[,,] BoardAsBytes(){

        int sx = data.GetLength(0);
        int sy = data.GetLength(1);
        int sz = data.GetLength(2);
        byte[,,] bytes = new byte[sx,sy,sz];
        foreach(Stone stone in data){
            if(stone.x < 000 || stone.y < 000 || stone.z < 000) continue;
            if(stone.x >= sx || stone.y >= sy || stone.z >= sz) continue;
            bytes[stone.x, stone.y, stone.z] = stone.val;
        }
        return bytes;
    }
    public void BoardFromBytes(byte[,,] bytes){
        int sx = bytes.GetLength(0);
        int sy = bytes.GetLength(1);
        int sz = bytes.GetLength(2);
        for(int x = 0; x < sx; x++){
            for(int y = 0; y < sy; y++){
                for(int z = 0; z < sz; z++){
                    SetStoneValue(x, y, z, bytes[x,y,z]);
                }
            }   
        }
    }

    // converts:
    //    a-z to 0-25
    //    A-Z to 26-51 
    public int SGFtoXY(char c){
        int i = (int)c;
        if(i > 96) return i - 97;
        if(i > 64) return i - 39;
        return 0;
    }
    public void ApplySGF(string sgf, byte val){
        for (int i = 0; i < sgf.Length - 1; i+=2) {
            int x = SGFtoXY(sgf[i]);
            int y = SGFtoXY(sgf[i+1]);
            SetStoneValue(x, y, 0, val);
        }
    }
    public void PlayMovesOGS() {
        int [,] moves = { { 15, 3, -1 }, { 3, 15, -1 }, { 3, 3, -1 }, { 9, 9, -1 }, { 15, 15, -1 }, { 12, 13, -1 }, { 15, 9, -1 }, { 6, 5, -1 }, { 12, 12, -1 }, { 11, 12, -1 }, { 12, 11, -1 }, { 12, 10, -1 }, { 13, 10, -1 }, { 13, 9, -1 }, { 14, 9, -1 }, { 12, 5, -1 }, { 12, 9, -1 }, { 13, 8, -1 }, { 11, 10, -1 }, { 11, 11, -1 }, { 13, 7, -1 }, { 12, 8, -1 }, { 12, 7, -1 }, { 11, 9, -1 }, { 12, 10, -1 }, { 11, 7, -1 }, { 14, 8, -1 }, { 12, 6, -1 }, { 11, 8, -1 }, { 10, 8, -1 }, { 10, 9, -1 }, { 10, 10, -1 }, { 7, 5, -1 }, { 11, 9, -1 }, { 6, 6, -1 }, { 12, 8, -1 }, { 6, 4, -1 }, { 5, 5, -1 }, { 5, 4, -1 }, { 4, 5, -1 }, { 4, 4, -1 }, { 3, 5, -1 }, { 3, 4, -1 }, { 2, 5, -1 }, { 2, 4, -1 }, { 1, 5, -1 }, { 1, 4, -1 }, { 0, 4, -1 }, { 0, 3, -1 }, { 0, 5, -1 }, { 5, 6, -1 }, { 3, 6, -1 }, { 3, 7, -1 }, { 4, 7, -1 }, { 11, 13, -1 }, { 10, 11, -1 }, { 10, 12, -1 }, { 9, 10, -1 }, { 13, 13, -1 }, { 12, 14, -1 }, { 13, 14, -1 }, { 12, 15, -1 }, { 13, 15, -1 }, { 12, 16, -1 }, { 13, 16, -1 }, { 12, 17, -1 }, { 13, 17, -1 }, { 10, 13, -1 }, { 13, 8, -1 }, { 9, 12, -1 }, { 11, 8, -1 }, { 10, 9, -1 }, { 12, 8, -1 }, { 11, 14, -1 }, { 12, 18, -1 }, { 11, 18, -1 }, { 13, 12, -1 }, { 14, 3, -1 }, { 14, 4, -1 }, { 14, 2, -1 }, { 15, 2, -1 }, { 14, 1, -1 }, { 15, 1, -1 }, { 15, 0, -1 }, { 16, 0, -1 }, { 14, 0, -1 }, { 13, 3, -1 }, { 13, 2, -1 }, { 12, 2, -1 }, { 12, 1, -1 }, { 11, 1, -1 }, { 12, 3, -1 }, { 11, 2, -1 }, { 13, 4, -1 }, { 11, 3, -1 }, { 11, 4, -1 }, { 10, 3, -1 }, { 9, 3, -1 }, { 10, 4, -1 }, { 10, 5, -1 }, { 9, 4, -1 }, { 8, 4, -1 }, { 9, 2, -1 }, { 8, 3, -1 }, { 7, 4, -1 }, { 8, 5, -1 }, { 7, 6, -1 }, { 9, 5, -1 }, { 8, 2, -1 }, { 9, 1, -1 }, { 8, 1, -1 }, { 10, 1, -1 }, { 10, 0, -1 }, { 11, 0, -1 }, { 12, 0, -1 }, { 13, 0, -1 }, { 9, 0, -1 }, { 11, 0, -1 }, { 10, 2, -1 }, { 12, 0, -1 }, { 4, 6, -1 }, { 3, 8, -1 }, { 2, 7, -1 }, { 2, 8, -1 }, { 2, 6, -1 }, { 1, 7, -1 }, { 7, 3, -1 }, { 5, 1, -1 }, { 5, 2, -1 }, { 6, 1, -1 }, { 6, 2, -1 }, { 7, 0, -1 }, { 4, 1, -1 }, { 4, 0, -1 }, { 3, 0, -1 }, { 5, 0, -1 }, { 3, 2, -1 }, { 6, 8, -1 }, { 6, 9, -1 }, { 6, 7, -1 }, { 5, 7, -1 }, { 5, 8, -1 }, { 4, 8, -1 }, { 1, 6, -1 }, { 4, 7, -1 }, { 4, 9, -1 }, { 5, 9, -1 }, { 7, 8, -1 }, { 3, 9, -1 }, { 4, 10, -1 }, { 2, 9, -1 }, { 1, 8, -1 }, { 1, 9, -1 }, { 0, 9, -1 }, { 0, 10, -1 }, { 1, 10, -1 }, { 0, 8, -1 }, { 2, 10, -1 }, { 0, 7, -1 }, { 0, 11, -1 }, { 0, 6, -1 }, { 0, 9, -1 }, { 8, 6, -1 }, { 9, 6, -1 }, { 8, 7, -1 }, { 8, 8, -1 }, { 9, 7, -1 }, { 10, 7, -1 }, { 9, 8, -1 }, { 8, 9, -1 }, { 3, 14, -1 }, { 3, 13, -1 }, { 4, 15, -1 }, { 4, 14, -1 }, { 2, 14, -1 }, { 2, 15, -1 }, { 4, 13, -1 }, { 5, 14, -1 }, { 4, 12, -1 }, { 3, 12, -1 }, { 3, 11, -1 }, { 2, 12, -1 }, { 2, 11, -1 }, { 3, 10, -1 }, { 1, 12, -1 }, { 4, 11, -1 }, { 2, 13, -1 }, { 5, 12, -1 }, { 1, 15, -1 }, { 5, 15, -1 }, { 4, 16, -1 }, { 3, 16, -1 }, { 3, 17, -1 }, { 4, 17, -1 }, { 2, 16, -1 }, { 5, 16, -1 }, { 4, 18, -1 }, { 5, 17, -1 }, { 5, 13, -1 }, { 6, 13, -1 }, { 5, 10, -1 }, { 5, 11, -1 }, { 6, 11, -1 }, { 7, 10, -1 }, { 6, 12, -1 }, { 6, 10, -1 }, { 6, 14, -1 }, { 7, 13, -1 }, { 6, 15, -1 }, { 7, 15, -1 }, { 6, 16, -1 }, { 6, 17, -1 }, { 7, 17, -1 }, { 7, 16, -1 }, { 5, 18, -1 }, { 7, 14, -1 }, { 6, 18, -1 }, { 8, 17, -1 }, { 7, 18, -1 }, { 2, 18, -1 }, { 1, 18, -1 }, { 8, 18, -1 }, { 3, 18, -1 }, { 0, 10, -1 }, { 0, 12, -1 }, { 7, 9, -1 }, { 16, 12, -1 }, { 15, 4, -1 }, { 16, 4, -1 }, { 14, 5, -1 }, { 15, 5, -1 }, { 16, 1, -1 }, { 16, 2, -1 }, { 17, 0, -1 }, { 17, 2, -1 }, { 16, 5, -1 }, { 14, 4, -1 }, { 15, 6, -1 }, { 15, 4, -1 }, { 17, 4, -1 }, { 17, 3, -1 }, { 17, 5, -1 }, { 13, 6, -1 }, { 17, 1, -1 }, { 18, 1, -1 }, { 16, 0, -1 }, { 18, 3, -1 }, { 18, 0, -1 }, { 13, 5, -1 }, { 14, 6, -1 }, { 15, 7, -1 }, { 16, 6, -1 }, { 16, 7, -1 }, { 17, 7, -1 }, { 17, 8, -1 } };

        for(int i = 0; i < moves.GetLength(0); i++) {
            PlayStoneAt(moves[i,0], moves[i,1], 0);
        }

    }

    /// <summary>
    /// Toggles whose turn it is.
    /// </summary>
    public void NextTurn()
    {

        currentPlayerTurn++;
        if (currentPlayerTurn > numberOfPlayers) currentPlayerTurn = 1;
        stonesRemaining = stonesPerPlay;

        // update displays:

        goban.ChangePlayerTurn(currentPlayerTurn);
        dummyObject.ChangePlayerTurn(currentPlayerTurn);

        // OPPONENT_AUTO_TAKE_TURN
        if(currentPlayerTurn != localPlayer && move_tree != null){
            if(move_tree.branches == null || move_tree.branches.Length == 0){
                //Debug.Log("No info for ai");
            } else {
                var nextMove = move_tree.branches[0];
                if(nextMove != null) PlayStoneAt(nextMove.x, nextMove.y);
            }
        }
        goban.Display(data);
    }
    public bool HasHistory {
        get {
            if(history == null) return false;
            return (history.Count > 0);
        }
    }
    public void PrevTurn()
    {

        if(!HasHistory) return;

        currentPlayerTurn--;
        if(currentPlayerTurn < 1) currentPlayerTurn = numberOfPlayers;

        Move lastMove = history[history.Count - 1];
        history.Remove(lastMove);

        move_tree = lastMove.pre_tree;
        if(lastMove.pre_stones != null){
            // reset the board to the stored state:
            BoardFromBytes(lastMove.pre_stones);
        } else {
            // at least remove the placed stone:
            SetStoneValue(lastMove.x, lastMove.y, lastMove.z, 0);
        }
        goban.ChangePlayerTurn(currentPlayerTurn);
        dummyObject.ChangePlayerTurn(currentPlayerTurn);
        goban.Display(data);
    }
    /// <summary>
    /// Changes a designated stone's value (in other words, "play a stone"). No rule-checking is done.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="val"></param>
    private void SetStoneValue(int x, int y, int z, byte val)
    {
        Stone s = GetStoneAt(x, y, z);
        if(s != null) s.val = val; // find the stone in question
        else {
            Debug.Log($"[ {x}, {y}, {z} ] is NULL");
        }
    }
    public bool IsValidSpot(int x, int y, int z = 0)
    {
        if (x < 0 || y < 0 || z < 0) return false;
        if (x >= data.GetLength(0) || y >= data.GetLength(1) || z >= data.GetLength(2)) return false;
        return true;
    }
    public bool IsVacantSpot(int x, int y, int z)
    {
        if (!IsValidSpot(x, y)) return false;
        if (data[x, y, z].val != 0) return false;
        return true;
    }
    /// <summary>
    /// Detects suicides, ko, and captures. Any detected captures are immediately removed from the game.
    /// </summary>
    /// <returns>If successful, return true. False means a suicide or ko is detected.</returns>
    bool CaptureStones()
    {
        List<GroupOfStones> groups = FindAllGroups();
        bool areThereAnyCaptures = CheckForCaptures(groups);
        bool areThereAnySuicides = CheckForSuicides(groups);
        if (!areThereAnyCaptures && areThereAnySuicides) return false; // ILLEGAL MOVE: committed suicide

        // TODO: check for Ko, return false; // ILLEGAL MOVE: ko

        // destroy all captured pieces
        RemoveCapturedGroups(groups);

        // rescan the board to get accurate liberty counts...

        // show ataris

        return true;
    }
    /// <summary>
    /// This function removes all groups with 0 liberties, except for groups owned by the current player.
    /// </summary>
    /// <param name="groups">The list of current groups</param>
    void RemoveCapturedGroups(List<GroupOfStones> groups)
    {
        foreach (GroupOfStones group in groups)
        {
            if (group.liberties.Count > 0) continue; // ignore groups that have liberties
            if (group.val == currentPlayerTurn) continue; // ignore current player's stones
            foreach (Stone stone in group.stones)
            {
                stone.val = 0;
            }
        }
    }
    /// <summary>
    /// Checks a list of groups to see if any groups of a particular player are captured.
    /// </summary>
    /// <param name="groups">The list of groups to check (should be all the groups on the board).</param>
    /// <param name="currentPlayerTurn">Which player value to check. 1/2 for black/white.</param>
    /// <returns></returns>
    bool CheckForCaptures(List<GroupOfStones> groups)
    {
        foreach(GroupOfStones group in groups)
        {
            if (group.val == currentPlayerTurn) continue;
            if (group.liberties.Count == 0) return true;
        }
        return false;
    }
    bool CheckForSuicides(List<GroupOfStones> groups)
    {
        foreach (GroupOfStones group in groups)
        {
            if (group.val != currentPlayerTurn) continue;
            if (group.liberties.Count == 0) return true;
        }
        return false;
    }
    List<GroupOfStones> FindAllGroups()
    {
        foreach (Stone stone in data) stone.inGroup = false;

        List<GroupOfStones> groups = new List<GroupOfStones>();
        foreach(Stone stone in data)
        {
            GroupOfStones group = MakeGroup(stone);
            if (group != null && group.stones.Count > 0)
            {
                groups.Add(group);
            }
        }
        return groups;
    }
    GroupOfStones MakeGroup(Stone stone)
    {
        if (stone == null) return null; // stone is null
        if (stone.inGroup) return null; // stone is already in a group
        if (stone.val == 0) return null; // open space (liberty)

        GroupOfStones group = new GroupOfStones(stone.val);
        AddToGroup(group, stone);

        return group;
    }
    void AddToGroup(GroupOfStones group, Stone stone)
    {
        if (stone == null) return; // stone is null
        if (stone.inGroup) return; // stone is already in a group
        if (stone.val == 0)
        {
            group.AddLiberty(stone);
            return; // open space (liberty)
        }

        int x = stone.x;
        int y = stone.y;
        int z = stone.z;

        if (group.AddStone(stone))
        {
            AddToGroup(group, GetStoneAt(x + 1, y, z));
            AddToGroup(group, GetStoneAt(x - 1, y, z));
            AddToGroup(group, GetStoneAt(x, y + 1, z));
            AddToGroup(group, GetStoneAt(x, y - 1, z));
            AddToGroup(group, GetStoneAt(x, y, z + 1));
            AddToGroup(group, GetStoneAt(x, y, z - 1));
        }
    }
    Stone GetStoneAt(int x, int y, int z)
    {
        if (!IsValidSpot(x, y, z)) return null;
        return data[x, y, z];
    }

    
    public byte[,,] GetData()
    {
        int sx = this.data.GetLength(0);
        int sy = this.data.GetLength(1);
        int sz = this.data.GetLength(2);

        byte[,,] data = new byte[sx, sy, sz];
        for (byte x = 0; x < sx; x++)
        {
            for (byte y = 0; y < sy; y++)
            {
                for (byte z = 0; z < sz; z++)
                {
                    data[x, y, z] = GetStoneAt(x, y, z).val;
                }
            }
        }
        return data;
    }
    public void SetData(byte[,,] data)
    {
        int sx = this.data.GetLength(0);
        int sy = this.data.GetLength(1);
        int sz = this.data.GetLength(2);

        this.data = new Stone[sx, sy, sz];
        for (byte x = 0; x < sx; x++)
        {
            for (byte y = 0; y < sy; y++)
            {
                for (byte z = 0; z < sz; z++)
                {
                    this.data[x, y, z] = new Stone(x, y, z);
                    this.data[x, y, z].val = data[x, y, z];
                }
            }
        }
    }

}


[CustomEditor(typeof(PlayController))]
public class EditorPlayController: Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Play Local Game")) (target as PlayController).BeginGame();

        //if (GUILayout.Button("Play OGS Game")) (target as PlayController).PlayMovesOGS();
    }
    
}