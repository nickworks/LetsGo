using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    float pitch = 0;
    float yaw = 0;

    public float sensitivityX = 1;
    public float sensitivityY = 1;
    public float sensitivityScroll = 1;

    public bool invertLookX = false;
    public bool invertLookY = true;
    public bool invertScroll = false;
    public float moveEasing = 1;
    public float dollyEasing = 5;
    public float maxCamDistance = 10;
    public float minCamDistance = 5;

    //public Vector3 target;
    public PlayController play;
    float dollyTarget = 5;
    Transform cam;
    

    void Start()
    {
        cam = GetComponentInChildren<Camera>().transform;
    }

	// Update is called once per frame
	void Update () {

        if(!Input.GetButton("Jump")) dollyTarget += Input.mouseScrollDelta.y * sensitivityScroll * (invertScroll ? -1 : 1);
        dollyTarget = Mathf.Clamp(dollyTarget, -maxCamDistance, -minCamDistance);

        if (Input.GetButton("Fire2"))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw += mx * sensitivityX * (invertLookX ? - 1: 1);
            pitch += my * sensitivityY * (invertLookY ? -1 : 1);
        }
        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
        transform.position = Vector3.Lerp(transform.position, play.GetCenter(), Time.deltaTime * moveEasing);
        cam.localPosition = Vector3.Lerp(cam.localPosition, new Vector3(0, 0, dollyTarget), Time.deltaTime * dollyEasing);
    }
}
