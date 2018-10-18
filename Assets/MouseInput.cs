﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {

    public Camera cam;
    public StoneController prefabStone;
    public Transform dummy;
    PlayController play;
    int z = 0;

    public bool meshMode = false;

    void Start () {
        play = GetComponent<PlayController>();
    }
    void Update()
    {
        if (meshMode) MeshUpdate();
        else GobanUpdate();
    }

    private void MeshUpdate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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

    void GobanUpdate()
    {
        if (!play.goban) return;

        if(Input.GetButton("Jump")) z += (int)(Input.mouseScrollDelta.y);
        z = Mathf.Clamp(z, 0, play.SizeZ() - 1);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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
