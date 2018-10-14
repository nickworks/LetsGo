using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobanRenderer : MonoBehaviour {

    StoneController[,,] stones;
    
    public StoneController prefabStone;
    public Transform prefabLine;

    void SpawnStones(int sizex, int sizey, int sizez)
    {
        stones = new StoneController[sizex, sizey, sizez];
        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                for (int z = 0; z < sizez; z++)
                {
                    Vector3 pos = new Vector3(x, z, y);
                    stones[x, y, z] = Instantiate(prefabStone, pos, Quaternion.identity);
                }
            }
        }
    }
    void SpawnLines(int sizex, int sizey, int sizez)
    {

        int x = sizex - 1;
        int y = sizey - 1;
        int z = sizez - 1;

        Instantiate(prefabLine, new Vector3(0, 0, 0), Quaternion.identity).localScale = Vector3.one * y;
        Instantiate(prefabLine, new Vector3(x, 0, 0), Quaternion.identity).localScale = Vector3.one * y;
        Instantiate(prefabLine, new Vector3(x, z, 0), Quaternion.identity).localScale = Vector3.one * y;
        Instantiate(prefabLine, new Vector3(0, z, 0), Quaternion.identity).localScale = Vector3.one * y;
        
        Quaternion rotateYaw = Quaternion.Euler(0, 90, 0);
        Instantiate(prefabLine, new Vector3(0, 0, 0), rotateYaw).localScale = Vector3.one * x;
        Instantiate(prefabLine, new Vector3(0, 0, y), rotateYaw).localScale = Vector3.one * x;
        Instantiate(prefabLine, new Vector3(0, z, y), rotateYaw).localScale = Vector3.one * x;
        Instantiate(prefabLine, new Vector3(0, z, 0), rotateYaw).localScale = Vector3.one * x;
        
        Quaternion rotatePitch = Quaternion.Euler(90, 0, 0);
        Instantiate(prefabLine, new Vector3(x, z, 0), rotatePitch).localScale = Vector3.one * z;
        Instantiate(prefabLine, new Vector3(x, z, y), rotatePitch).localScale = Vector3.one * z;
        Instantiate(prefabLine, new Vector3(0, z, y), rotatePitch).localScale = Vector3.one * z;
        Instantiate(prefabLine, new Vector3(0, z, 0), rotatePitch).localScale = Vector3.one * z;
        
    }
    public void Display(PlayController.Stone[,,] board, bool random = false)
    {
        int sizex = board.GetLength(0);
        int sizey = board.GetLength(1);
        int sizez = board.GetLength(2);
        if (stones == null)
        {
            SpawnStones(sizex, sizey, sizez);
            SpawnLines(sizex, sizey, sizez);
        }

        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                for (int z = 0; z < sizez; z++)
                {
                    byte val = board[x, y, z].val;
                    if (random) val = (byte)((Random.value * 3) % 3);
                    stones[x, y, z].SetGameState(val);
                }
            }
        }
        //ghost.SetGameState((byte)(isPlayer1Turn ? 1 : 2));
    }
    int highlightedLayer = -1;

    public void HighlightLayer(int tx, int ty, int tz, byte value)
    {
        //if (highlightedLayer == tx) return;
        //highlightedLayer = tx;

        int sizex = stones.GetLength(0);
        int sizey = stones.GetLength(1);
        int sizez = stones.GetLength(2);

        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                for (int z = 0; z < sizez; z++)
                {
                    stones[x, y, z].Highlight(tz == z, tx == x && ty == y && tz == z);
                    stones[x, y, z].PreviewGameState(value);
                }
            }
        }
    }

    
}
