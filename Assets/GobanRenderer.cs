using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobanRenderer : MonoBehaviour {

    PieceController[,] stones;
    
    public PieceController prefabStone;
    public Transform prefabLine;

    void SpawnStones(int sizex, int sizey)
    {
        stones = new PieceController[sizex, sizey];
        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                stones[x, y] = Instantiate(prefabStone, pos, Quaternion.identity);
                stones[x, y].transform.localScale = Vector3.one * 0.6f;
            }
        }
    }
    void SpawnLines(int sizex, int sizey)
    {
        for (int x = 0; x < sizex; x++)
        {
            Instantiate(prefabLine, new Vector3(x, 0, 0), Quaternion.identity);
        }
        Quaternion rotateYaw = Quaternion.Euler(0, 90, 0);
        for (int y = 0; y < sizey; y++)
        {
            Instantiate(prefabLine, new Vector3(0, 0, y), rotateYaw);
        }
    }
    public void Display(PlayController.Stone[,] board, bool random = false)
    {
        int sizex = board.GetLength(0);
        int sizey = board.GetLength(1);
        if (stones == null)
        {
            SpawnStones(sizex, sizey);
            SpawnLines(sizex, sizey);
        }

        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                byte val = board[x, y].val;
                if (random) val = (byte)((Random.value * 3) % 3);
                stones[x, y].SetGameState(val);
            }
        }
        //ghost.SetGameState((byte)(isPlayer1Turn ? 1 : 2));
    }
}
