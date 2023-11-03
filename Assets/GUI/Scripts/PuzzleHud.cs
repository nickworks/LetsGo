using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleHud : MonoBehaviour
{
    public void Undo(){
        PlayController.singleton.PrevTurn();
    }
}
