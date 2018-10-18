using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobanRenderer : MonoBehaviour {

    StoneController[,,] stones;
    
    public StoneController prefabStone;
    public Transform prefabLine;

    List<LineRenderer>[] lines; // an array of lists

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

        lines = new List<LineRenderer>[sizez];

        Quaternion rotateYaw = Quaternion.Euler(0, 90, 0);
        Quaternion rotatePitch = Quaternion.Euler(-90, 0, 0);

        for (int z = 0; z < sizez; z++)
        {
            List<LineRenderer> bunchOfLines = new List<LineRenderer>();
            lines[z] = bunchOfLines;
            for (int x = 0; x < sizex; x++)
            {
                Transform obj = Instantiate(prefabLine, new Vector3(0, z, x), rotateYaw);
                lines[z].Add(obj.GetComponent<LineRenderer>()); // here be glitches?
                obj.localScale = Vector3.one * (sizex - 1);
            }

            for (int y = 0; y < sizey; y++)
            {
                Transform obj = Instantiate(prefabLine, new Vector3(y, z, 0), Quaternion.identity);
                lines[z].Add(obj.GetComponent< LineRenderer>());
                obj.localScale = Vector3.one * (sizey - 1);
            }
        }
        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                Instantiate(prefabLine, new Vector3(y, 0, x), rotatePitch).localScale = Vector3.one * (sizez - 1);
            }
        }

        /*
        sizex--;
        sizey--;
        sizez--;
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
        */

    }
    /// <summary>
    /// Loops through the stones models and updates their state.
    /// </summary>
    /// <param name="board">The state of the board, a multi-dimensional array of Stone objects.</param>
    public void Display(PlayController.Stone[,,] board)
    {
        //print("Model updating state now...");
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
                    stones[x, y, z].SetGameState( val ); // update every stone
                }
            }
        }
        //ghost.SetGameState((byte)(isPlayer1Turn ? 1 : 2));
    }

    StoneController highlight;
    StoneController GetStone(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0) return null;
        if (x >= stones.GetLength(0) || y >= stones.GetLength(1) || z >= stones.GetLength(2)) return null;
        return stones[x, y, z];
    }
    int previewX = 0;
    int previewY = 0;
    int previewZ = 0;

    public void Highlight(int tx, int ty, int tz, byte whosTurn)
    {
        if (previewX == tx && previewY == ty && previewZ == tz) return;
        previewX = tx; previewY = ty; previewZ = tz;

        int sizex = stones.GetLength(0);
        int sizey = stones.GetLength(1);
        int sizez = stones.GetLength(2);

        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                for (int z = 0; z < sizez; z++)
                {
                    StoneController stone = stones[x, y, z];
                    if (stone.value > 0) continue;
                    stone.gameObject.SetActive(tz == z);
                    stone.showLargePreview = (tx == x && ty == y && tz == z);
                    //stones[x, y, z].PreviewGameState(whosTurn);
                }
            }
        }
        
    }
    public void ChangePlayerTurn(int val)
    {
        int sizex = stones.GetLength(0);
        int sizey = stones.GetLength(1);
        int sizez = stones.GetLength(2);
        for (int x = 0; x < sizex; x++)
        {
            for (int y = 0; y < sizey; y++)
            {
                for (int z = 0; z < sizez; z++)
                {
                    StoneController stone = stones[x, y, z];
                    if (stone.value > 0) continue;
                    stones[x, y, z].PreviewGameState(val);
                }
            }
        }
    }
    
}
