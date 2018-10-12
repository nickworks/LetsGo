using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour {

    public class Stone {
        public Stone(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }
        public byte x;
        public byte y;
        public byte val = 0;
        public bool inGroup = false;
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
                if (temp.x == loc.x && temp.y == loc.y) return true;
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
                if (temp.x == loc.x && temp.y == loc.y) return true;
            }
            return false;
        }
    }

    Stone[,] board = new Stone[9, 9];
    bool isPlayer1Turn = true;
    PieceController ghost;

    public Camera cam;

    GobanRenderer goban;
    public PieceController prefabStone;

    // Use this for initialization
    void Start () {
        ClearBoard();
        goban = GetComponent<GobanRenderer>();
        goban.Display(board);
        ghost = Instantiate(prefabStone);
        ghost.isGhost = true;
    }

    // Update is called once per frame
    void Update () {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
        float dis = 0;
        if(plane.Raycast(ray, out dis))
        {
            Vector3 pos = ray.GetPoint(dis);
            int x = (int)Mathf.Round(pos.x);
            int y = (int)Mathf.Round(pos.z);
            if (IsVacantSpot(x, y))
            {
                ghost.gameObject.SetActive(true);
                ghost.transform.position = new Vector3(x, 0, y);
                if (Input.GetButtonDown("Fire1"))
                {
                    if (SetPiece(x, y, (byte)(isPlayer1Turn ? 1 : 2)))
                    {
                        isPlayer1Turn = !isPlayer1Turn;
                        goban.Display(board);
                        ghost.gameObject.SetActive(false);
                        ghost.SetGameState((byte)(isPlayer1Turn ? 1 : 2));
                    }
                }
            } else
            {
                ghost.gameObject.SetActive(false);
            }
            
        }
	}
    /// <summary>
    /// Sets every stone's value to 0.
    /// </summary>
    void ClearBoard()
    {
        for (byte x = 0; x < board.GetLength(0); x++)
        {
            for (byte y = 0; y < board.GetLength(1); y++)
            {
                board[x, y] = new Stone(x, y);
            }
        }
    }
    
    bool SetPiece(int x, int y, byte val)
    {
        if (!IsVacantSpot(x, y)) return false;

        Stone stone = GetPiece(x, y);
        stone.val = val;
        if (CheckGroups()) {
            stone.val = 0;
            return false;
        }

        return true;
    }
    bool IsValidSpot(int x, int y)
    {
        if (x < 0 || y < 0) return false;
        if (x >= board.GetLength(0) || y >= board.GetLength(1)) return false;
        return true;
    }
    bool IsVacantSpot(int x, int y)
    {
        if (!IsValidSpot(x, y)) return false;
        if (board[x, y].val != 0) return false;
        return true;
    }
    bool CheckGroups()
    {
        List<GroupOfStones> groups = FindAllGroups();
        bool captures = CheckForCaptures(groups, (byte)(isPlayer1Turn ? 2 : 1));
        bool suicides = CheckForCaptures(groups, (byte)(isPlayer1Turn ? 1 : 2));
        if (!captures && suicides) return true; // committed suicide

        // check for Ko, return true

        // destroy all captured pieces
        RemoveCapturedGroups(groups);

        // rescan the board to get accurate liberty counts...

        // show ataris

        return false;
    }

    private void RemoveCapturedGroups(List<GroupOfStones> groups)
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

        if (group.AddStone(stone))
        {
            AddToGroup(group, GetPiece(x + 1, y));
            AddToGroup(group, GetPiece(x - 1, y));
            AddToGroup(group, GetPiece(x, y + 1));
            AddToGroup(group, GetPiece(x, y - 1));
        }
    }
    Stone GetPiece(int x, int y)
    {
        if (!IsValidSpot(x, y)) return null;
        return board[x, y];
    }
}
