using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    float pitch = 45;
    float yaw = 10;

    public float sensitivityX = 1;
    public float sensitivityY = 1;
    public float sensitivityScroll = 1;

    public bool invertLookX = false;
    public bool invertLookY = true;
    public bool invertScroll = false;
    public float moveEasing = 1;
    public float dollyEasing = 5;
    public float maxCamDistance = 20;
    public float minCamDistance = 2;

    //public Vector3 target;
    public PlayController play;
    float dollyTarget = -15;
    Transform cam;

    Vector3 offset = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    public int maxSpeed = 10;
    public float decelerationMultiplier = 10;
    public float accelerationMultiplier = 10;

    void Start()
    {
        cam = GetComponentInChildren<Camera>().transform;
    }

	// Update is called once per frame
	void Update ()
    {
        Dolly();
        Orbit();

        

        Ease();
    }

    private void Ease()
    {

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0 && v == 0) {
            velocity = Vector3.zero;
        } else
        {
            velocity += transform.forward * v * Time.deltaTime * accelerationMultiplier;
            velocity += transform.right * h * Time.deltaTime * accelerationMultiplier;
        }
        offset += velocity * Time.deltaTime;
        offset.y = 0;
        Vector3 pos = Vector3.zero + offset;
        if (play)
        {
            pos = play.GetCenter() + offset;
            pos = play.GetBounds().ClosestPoint(pos);
        }
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * moveEasing);
        
    }

    private void Orbit()
    {
        if (Input.GetButton("Fire2"))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw += mx * sensitivityX * (invertLookX ? -1 : 1);
            pitch += my * sensitivityY * (invertLookY ? -1 : 1);
        }
        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
        cam.localPosition = Vector3.Lerp(cam.localPosition, new Vector3(0, 0, dollyTarget), Time.deltaTime * dollyEasing);
    }

    private void Dolly()
    {
        if (!Input.GetButton("Jump")) dollyTarget += Input.mouseScrollDelta.y * sensitivityScroll * (invertScroll ? -1 : 1);
        dollyTarget = Mathf.Clamp(dollyTarget, -maxCamDistance, -minCamDistance);
    }
}
