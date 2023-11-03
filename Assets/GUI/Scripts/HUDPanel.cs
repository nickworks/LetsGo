using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPanel : MonoBehaviour {
    
    public void Back() {
        
    }
    public void Undo() {
        PlayController.singleton.PrevTurn();
    }
}
