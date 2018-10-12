using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class StoneController : MonoBehaviour {

    MeshRenderer view;
    public bool isGhost = false;

    public void SetPlayer(bool isPlayer1, bool isVisible = true)
    {
        int val = 0;
        if (isVisible) val = isPlayer1 ? 1 : 2;
        SetGameState(val);
    }
    public void SetGameState(int val)
    {
        if(!view) view = gameObject.GetComponent<MeshRenderer>();

        switch (val)
        {
            case 0:
                gameObject.SetActive(false);
                break;
            case 1:
                gameObject.SetActive(true);
                view.material.color = Color.black;
                break;
            case 2:
                gameObject.SetActive(true);
                view.material.color = Color.white;
                break;
        }
    }
}
