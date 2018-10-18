using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour {

    /// <summary>
    /// How many seconds to sleep when Sleep() is called.
    /// </summary>
    public float invisibleTimeAmount = 1;
    /// <summary>
    /// How many seconds to fade-in the object after sleeping.
    /// </summary>
    public float fadeInTimeAmount = .01f;
    float invisibleTimer;
    float fadeInTimer;
    MeshRenderer mesh;
    
    void Start () {
        mesh = GetComponent<MeshRenderer>();
    }
    public void Sleep()
    {
        invisibleTimer = invisibleTimeAmount;
        fadeInTimer = fadeInTimeAmount;
        transform.localScale = Vector3.zero;
    }
	public void ChangePlayerTurn(byte whoseTurn)
    {
        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        if (whoseTurn == 1) mesh.material.color = Color.black;
        if (whoseTurn == 2) mesh.material.color = Color.white;
        Sleep();
    }
	void Update () {
        if (invisibleTimer > 0) invisibleTimer -= Time.deltaTime;
        else if (fadeInTimer > 0)
        {
            fadeInTimer -= Time.deltaTime;
            float percent = 1 - (fadeInTimer / fadeInTimeAmount);
            transform.localScale = Vector3.one * percent;
        }
        
	}
}
