using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : MonoBehaviour {

    class Stone {
        public byte x;
        public byte y;
        public byte val;
        public bool inGroup;
    }
    struct GroupOfStones
    {
        Stone[] stones;
        Stone[] liberties;
        int val;
        int countLiberties { get { return liberties == null ? 0 : liberties.Length; } }
        bool AddStone(Stone stone)
        {
            if (HasStone(stone)) return false;

            List<Stone> temp = new List<Stone>(stones);
            temp.Add(stone);
            stones = temp.ToArray();
            stone.inGroup = true;

            return true;
        }
        bool HasStone(Stone loc)
        {
            foreach(Stone temp in stones)
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
        if (false){//!DidPlayerCapture() && DidPlayerSuicide()) {
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
    bool CheckForCaptures(byte val)
    {

        return false;
    }
    void FindAllGroups()
    {

    }
    List<Stone> GetMatchingNeighbors(int x, int y)
    {
        // TODO: need a closed list & an open list
        // TODO: make this a recursive function... maybe?
        Stone stone = GetPiece(x, y);
        List<Stone> res = new List<Stone>();

        // GetMatchingNeighbors()
        return res;
    }
    Stone GetPiece(int x, int y)
    {
        if (!IsValidSpot(x, y)) return null;
        return board[x, y];
    }
}
