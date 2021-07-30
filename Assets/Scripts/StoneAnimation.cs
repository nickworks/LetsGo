using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAnimation : MonoBehaviour {

    float dropSpeed = 10.5f;
    float maxSpeed = 800.0f;
    float stoneSize = .5f;

    bool animComplete = false;
    MeshRenderer mesh;

    void Start () {
        mesh = GetComponent<MeshRenderer>();
	}	
	void Update () {
        if (animComplete) return;

        DropToTheBoard();

    }
    void DropToTheBoard()
    {
        dropSpeed += 20.0f * Time.deltaTime;
        if (dropSpeed > maxSpeed) dropSpeed = maxSpeed;
        transform.localPosition += Vector3.down * dropSpeed * Time.deltaTime;
        if (transform.localPosition.y <= 0)
        {
            transform.localPosition = Vector3.zero;
            animComplete = true;
        }

        stoneSize = Mathf.Lerp(stoneSize, .75f, Time.deltaTime * 7);
        transform.localScale = stoneSize * new Vector3(1, .4f, 1);
    }
}
