using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {

    public Camera cam;
    
    public StoneController prefabStone;

    StoneController ghost;
    PlayController play;
    GobanRenderer goban;

    int z = 0;

    public Transform dummy;

    void Start () {
        play = GetComponent<PlayController>();
        goban = GetComponent<GobanRenderer>();
        ghost = Instantiate(prefabStone);
        ghost.SetIsGhost(true);
    }

    // Update is called once per frame
    void Update()
    {

        z += (int)(Input.mouseScrollDelta.y);
        z = Mathf.Clamp(z, 0, play.SizeZ() - 1);
        

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, -z);
        
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
        float dis = 0;

        bool showGhost = false;
        if (plane.Raycast(ray, out dis))
        {
            Vector3 pos = ray.GetPoint(dis);
            int x = (int)Mathf.Round(pos.x);
            int y = (int)Mathf.Round(pos.z);

            if(dummy)dummy.position = pos;

            goban.HighlightLayer(x, y, z, (byte)(play.isPlayer1Turn ? 1 : 2));

            if (play.IsVacantSpot(x, y, z))
            {
                showGhost = true;
                ghost.transform.position = new Vector3(x, z, y);
                if (Input.GetButtonDown("Fire1"))
                {
                    if (play.PlayStoneAt(x, y, z)) showGhost = false;
                }
            }
        }
        //ghost.SetPlayer(play.isPlayer1Turn, showGhost);
    }
}
