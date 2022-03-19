using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    public static PanelSwitcher singleton {get; private set;}
    PuzzlePanel puzzlePanel;
    HUDPanel hudPanel;
    void Start()
    {
        if(singleton != null){
            Destroy(gameObject);
        }
        singleton = this;

        puzzlePanel = GetComponentInChildren<PuzzlePanel>();
        hudPanel = GetComponentInChildren<HUDPanel>();
        ShowPuzzle();
    }
    void OnDestroy(){
        if(singleton == this) singleton = null;
    }
    public void ShowPuzzle(){

        puzzlePanel.gameObject.SetActive(true);
        hudPanel.gameObject.SetActive(false);
    }
    public void ShowHUD(){

        puzzlePanel.gameObject.SetActive(false);
        hudPanel.gameObject.SetActive(true);
    }
}
