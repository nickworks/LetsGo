using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    float pitch = 0;
    float yaw = 0;

    public float sensitivityX = 1;
    public float sensitivityY = 1;

    public bool invertLookX = false;
    public bool invertLookY = true;
    public float moveEasing = 1;

    //public Vector3 target;
    public PlayController play;

    void Start()
    {
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetButton("Fire2"))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw += mx * sensitivityX * (invertLookX ? - 1: 1);
            pitch += my * sensitivityY * (invertLookY ? -1 : 1);
        }
        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
        transform.position = Vector3.Lerp(transform.position, play.GetCenter(), Time.deltaTime * moveEasing);
    }
}
