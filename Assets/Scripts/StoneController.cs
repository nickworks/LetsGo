using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneController : MonoBehaviour {

    MeshRenderer view;
    bool isGhost = false;
    public bool isHighlighting = false;
    public bool isSetValue = false;
    public bool isHovering = false;

    public Transform lines;

    int value = 0;

    public void SetPlayer(bool isPlayer1, bool isVisible = true)
    {
        int val = 0;
        if (isVisible) val = isPlayer1 ? 1 : 2;
        SetGameState(val);
    }
    public void SetGameState(int val)
    {
        value = val;
        if(value > 0) isSetValue = true;
    }
    public void PreviewGameState(int val)
    {
        if(!isSetValue) value = val;
    }
    public void SetIsGhost(bool isGhost)
    {
        this.isGhost = isGhost;
        lines.gameObject.SetActive(!isGhost);
    }
    void Update()
    {
        if (!view) view = GetComponentInChildren<MeshRenderer>();
        if (view)
        {
            //view.enabled = value > 0;
            view.enabled = true;

            if (value == 1) view.material.color = Color.black;
            if (value == 2) view.material.color = Color.white;

            float size = 0;
            
            if (isHighlighting) size = .05f;
            if (isHovering) size = .25f;
            if (isSetValue && value > 0) size = 1;

            lines.transform.localScale = Vector3.zero;// (value == 0 || isGhost) ? Vector3.zero : Vector3.one * .1f;

            view.transform.localScale = Vector3.Lerp(view.transform.localScale, Vector3.one * size, Time.deltaTime * 10);
                
        }
    }
    public void Highlight(bool highlight, bool hover)
    {
        isHighlighting = highlight;
        isHovering = hover;
    }
}
