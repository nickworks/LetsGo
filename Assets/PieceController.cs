using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {

    MeshRenderer view;
    public bool isGhost = false;

	void Start () {
        view = gameObject.GetComponent<MeshRenderer>();

    }
	void Update () {
		
	}
    public void SetGameState(byte val)

    {
        if(!view) view = gameObject.GetComponent<MeshRenderer>();
        if (isGhost) print(val + " player");
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
