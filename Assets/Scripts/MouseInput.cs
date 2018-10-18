using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {

    public Camera cam;
    public StoneController prefabStone;
    public Transform dummy;
    PlayController play;
    int z = 0;

    public bool isMouseMode = false;

    public bool meshMode = false;

    void Start () {
        play = GetComponent<PlayController>();
    }
    void Update()
    {
        CheckInputMode();
        Ray ray = GetSelectionRay();
        if (meshMode) MeshUpdate(ray);
        else GobanUpdate(ray);
    }

    private void CheckInputMode()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            isMouseMode = true;
        }
        if (Input.GetAxisRaw("Vertical2") != 0 || Input.GetAxisRaw("Horizontal2") != 0)
        {
            isMouseMode = false;
        }
        if (Input.GetAxisRaw("Triggers") != 0)
        {
            isMouseMode = false;
        }
    }

    /// <summary>
    /// This method sends out raycasts that look for collisions with the stone's colliders.
    /// </summary>
    private void MeshUpdate(Ray ray)
    {
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 10);

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity))
        {
            StoneController stone = hit.collider.GetComponent<StoneController>();
            if (stone.value == 0)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    stone.SetGameState(1);
                }
            }
        }
    }

    private Ray GetSelectionRay(bool draw = false)
    {
        Ray ray = isMouseMode
                    ? cam.ScreenPointToRay(Input.mousePosition)
                    : new Ray(cam.transform.position, cam.transform.forward);
        if(draw) Debug.DrawRay(ray.origin, ray.direction * 10);
        return ray;
    }

    /// <summary>
    /// This method sends out a raycast that checks for collision with an arbitrary ground plane. The ground plane is aligned with the z property.
    /// </summary>
    void GobanUpdate(Ray ray)
    {
        if (!play.goban) return;
        
        //move up or down "layers"

        if (Input.GetButton("Jump")) z += (int)(Input.mouseScrollDelta.y);
        if (Input.GetButtonDown("Bumpers")) z += (int)Input.GetAxisRaw("Bumpers");
        z = Mathf.Clamp(z, 0, play.SizeZ() - 1);

        Plane plane = new Plane(Vector3.up, -z);
        
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
        float dis = 0;

        if (plane.Raycast(ray, out dis))
        {
            Vector3 pos = ray.GetPoint(dis);
            int x = (int)Mathf.Round(pos.x);
            int y = (int)Mathf.Round(pos.z);

            if(dummy)dummy.position = pos;

            play.goban.Highlight(x, y, z, play.currentPlayerTurn);

            if (play.IsVacantSpot(x, y, z))
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    play.PlayStoneAt(x, y, z);
                }
            }
        }
    }
}
