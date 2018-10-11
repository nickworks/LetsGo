using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour {

    class Stone {
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
    class GroupOfStones
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
    PieceController[,] views;
    PieceController ghost;

    bool isPlayer1Turn = true;

    public PieceController prefabView;
    public Transform prefabLine;
    public Camera cam;
    
    // Use this for initialization
    void Start () {
        ClearBoard();
        BuildDisplay();
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
                        ghost.gameObject.SetActive(false);
                        RefreshDisplay();
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
    void BuildDisplay()
    {
        int sizex = board.GetLength(0);
        int sizey = board.GetLength(1);
        views = new PieceController[sizex,sizey];
        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                views[x, y] = Instantiate(prefabView, pos,Quaternion.identity);
                views[x, y].transform.localScale = Vector3.one * 0.6f;
            }
        }
        ghost = Instantiate(prefabView);
        ghost.isGhost = true;


        for (int x = 0; x < sizex; x++)
        {
            Instantiate(prefabLine, new Vector3(x, 0, 0), Quaternion.identity);
        }
        Quaternion rotateYaw = Quaternion.Euler(0, 90, 0);
        for (int y = 0; y < sizey; y++)
        {
            Instantiate(prefabLine, new Vector3(0, 0, y), rotateYaw);
        }

        RefreshDisplay();
    }
    void RefreshDisplay(bool random = false)
    {
        for (int x = 0; x < views.GetLength(0); x++)
        {
            for (int y = 0; y < views.GetLength(1); y++)
            {
                byte val = board[x, y].val;
                if (random) val = (byte) ((Random.value * 3) % 3);
                views[x, y].SetGameState(val);
            }
        }
        ghost.SetGameState((byte)(isPlayer1Turn ? 1 : 2));
    }
    bool SetPiece(int x, int y, byte val)
    {
        if (!IsVacantSpot(x, y)) return false;

        Stone stone = GetPiece(x, y);
        stone.val = val;
        if (ShouldUndo()) {
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
    void CheckForAtari()
    {

    }
    bool ShouldUndo()
    {
        List<GroupOfStones> groups = FindAllGroups();
        bool captures = CheckForCaptures(groups, (byte)(isPlayer1Turn ? 2 : 1));
        bool suicides = CheckForCaptures(groups, (byte)(isPlayer1Turn ? 1 : 2));
        return (!captures && suicides);
    }
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
