using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {

    public Camera cam;
    public PlayController play;

    public StoneController prefabStone;
    StoneController ghost;

    void Start () {
        play = GetComponent<PlayController>();
        ghost = Instantiate(prefabStone);
        ghost.isGhost = true;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
        float dis = 0;

        bool showGhost = false;
        if (plane.Raycast(ray, out dis))
        {
            Vector3 pos = ray.GetPoint(dis);
            int x = (int)Mathf.Round(pos.x);
            int y = (int)Mathf.Round(pos.z);
            if (play.IsVacantSpot(x, y))
            {
                showGhost = true;
                ghost.transform.position = new Vector3(x, 0, y);
                if (Input.GetButtonDown("Fire1"))
                {
                    if (play.PlayStoneAt(x, y)) showGhost = false;
                }
            }
        }
        ghost.SetPlayer(play.isPlayer1Turn, showGhost);
    }
}
