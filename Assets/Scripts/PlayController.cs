using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour {

    public class Stone {
        public Stone(byte x, byte y, byte z, byte val = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.val = 0;
        }
        public byte x;
        public byte y;
        public byte z;
        public byte val;
        public bool inGroup; // flag used during group formation
    }
    public class GroupOfStones
    {
        public GroupOfStones(int val)
        {
            stones = new List<Stone>();
            liberties = new List<Stone>();
            this.val = val;
        }
        public List<Stone> stones { get; private set; }
        public List<Stone> liberties { get; private set; }
        public int val { get; private set; }
        public bool AddStone(Stone stone)
        {
            if (stone.val != val) return false;
            if (HasStone(stone)) return false;
            stones.Add(stone);
            stone.inGroup = true;
            return true;
        }
        public bool HasStone(Stone loc)
        {
            foreach(Stone temp in stones)
            {
                if (temp.x == loc.x && temp.y == loc.y && temp.z == loc.z) return true;
            }
            return false;
        }
        public bool AddLiberty(Stone stone)
        {
            if (HasLiberty(stone)) return false;
            liberties.Add(stone);
            return true;
        }
        public bool HasLiberty(Stone loc)
        {
            foreach (Stone temp in liberties)
            {
                if (temp.x == loc.x && temp.y == loc.y && temp.z == loc.z) return true;
            }
            return false;
        }
    }

    Stone[,,] board = new Stone[9, 9, 2];

    public bool isPlayer1Turn { get; private set; }
    public int stonesPerPlay = 1;
    public int stonesRemaining = 0;
    GobanRenderer goban;

    // Use this for initialization
    void Start () {
        isPlayer1Turn = true;
        ClearBoard();
        goban = GetComponent<GobanRenderer>();
        goban.Display(board);
    }
    public Vector3 GetCenter() {
        return new Vector3(board.GetLength(0)-1, board.GetLength(2)-1, board.GetLength(1)-1) / 2;
    }
    public int SizeZ()
    {
        return board.GetLength(2);
    }
    /// <summary>
    /// Fills the board with stones, and sets every stone's value to 0.
    /// </summary>
    void ClearBoard()
    {
        for (byte x = 0; x < board.GetLength(0); x++)
        {
            for (byte y = 0; y < board.GetLength(1); y++)
            {
                for (byte z = 0; z < board.GetLength(2); z++)
                {
                    board[x, y, z] = new Stone(x, y, z);
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
        byte stoneVal = (byte)(isPlayer1Turn ? 1 : 2);

        SetStoneValue(x, y, z, stoneVal);

        if (CaptureStones()) // attempt to capture stones
        {
            stonesRemaining--;
            if(stonesRemaining <= 0) NextTurn();
            //print(stonesRemaining);
            goban.Display(board); // doesn't belong here?
            return true; // successful play
        }
        SetStoneValue(x, y, z, 0); // undo mode
        return false; // illegal move, no change
    }
    /// <summary>
    /// Toggles whose turn it is.
    /// </summary>
    public void NextTurn()
    {
        isPlayer1Turn = !isPlayer1Turn;
        stonesRemaining = stonesPerPlay;
        print(stonesPerPlay);
    }
    /// <summary>
    /// Changes a designated stone's value (in other words, "play a stone"). No rule-checking is done.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="val"></param>
    void SetStoneValue(int x, int y, int z, byte val)
    {
        GetStoneAt(x, y, z).val = val; // find the stone in question
    }
    public bool IsValidSpot(int x, int y, int z = 0)
    {
        if (x < 0 || y < 0 || z < 0) return false;
        if (x >= board.GetLength(0) || y >= board.GetLength(1) || z >= board.GetLength(2)) return false;
        return true;
    }
    public bool IsVacantSpot(int x, int y, int z)
    {
        if (!IsValidSpot(x, y)) return false;
        if (board[x, y, z].val != 0) return false;
        return true;
    }
    /// <summary>
    /// Detects suicides, ko, and captures. Any detected captures are immediately removed from the game.
    /// </summary>
    /// <returns>If successful, return true. False means a suicide or ko is detected.</returns>
    bool CaptureStones()
    {
        List<GroupOfStones> groups = FindAllGroups();
        bool areThereAnyCaptures = CheckForCaptures(groups, (byte)(isPlayer1Turn ? 2 : 1));
        bool areThereAnySuicides = CheckForCaptures(groups, (byte)(isPlayer1Turn ? 1 : 2));
        if (!areThereAnyCaptures && areThereAnySuicides) return false; // ILLEGAL MOVE: committed suicide

        // check for Ko, return true

        // destroy all captured pieces
        RemoveCapturedGroups(groups);

        // rescan the board to get accurate liberty counts...

        // show ataris

        return true;
    }

    void RemoveCapturedGroups(List<GroupOfStones> groups)
    {
        foreach (GroupOfStones group in groups)
        {
            if (group.liberties.Count > 0) continue;
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
    /// <param name="val">Which player value to check. 1/2 for black/white.</param>
    /// <returns></returns>
    bool CheckForCaptures(List<GroupOfStones> groups, byte val)
    {
        foreach(GroupOfStones group in groups)
        {
            if (group.val != val) continue;
            if (group.liberties.Count == 0) return true;
        }
        return false;
    }
    List<GroupOfStones> FindAllGroups()
    {
        foreach (Stone stone in board) stone.inGroup = false;

        List<GroupOfStones> groups = new List<GroupOfStones>();
        foreach(Stone stone in board)
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
        return board[x, y, z];
    }

    
    public byte[,,] GetData()
    {
        int sx = board.GetLength(0);
        int sy = board.GetLength(1);
        int sz = board.GetLength(2);

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
        int sx = board.GetLength(0);
        int sy = board.GetLength(1);
        int sz = board.GetLength(2);

        board = new Stone[sx, sy, sz];
        for (byte x = 0; x < sx; x++)
        {
            for (byte y = 0; y < sy; y++)
            {
                for (byte z = 0; z < sz; z++)
                {
                    board[x, y, z] = new Stone(x, y, z);
                    board[x, y, z].val = data[x, y, z];
                }
            }
        }
    }

}
