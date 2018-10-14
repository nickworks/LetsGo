using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneController : MonoBehaviour {

    MeshRenderer view;
    bool isGhost = false;
    public bool preventPreviewing = false;
    public bool showAsSelectable = false;
    public bool showPreview = false;

    public Transform lines;

    int value = 0;
    int previewValue = 0;

    public void SetPlayer(bool isPlayer1, bool isVisible = true)
    {
        int val = 0;
        if (isVisible) val = isPlayer1 ? 1 : 2;
        SetGameState(val);
    }
    public void SetGameState(int val)
    {
        value = val;
        preventPreviewing = (value > 0);
    }
    public void PreviewGameState(int val)
    {
        previewValue = val;
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

            

            float size = 0;
            if (preventPreviewing)
            {
                if (value == 1) view.material.color = Color.black;
                if (value == 2) view.material.color = Color.white;

                if (value > 0) size = 1;
            }
            else
            {
                if (previewValue == 1) view.material.color = Color.black;
                if (previewValue == 2) view.material.color = Color.white;

                if (showAsSelectable) size = .05f;
                if (showPreview) size = .25f;
            }
            lines.transform.localScale = Vector3.zero;// (value == 0 || isGhost) ? Vector3.zero : Vector3.one * .1f;

            view.transform.localScale = Vector3.Lerp(view.transform.localScale, Vector3.one * size, Time.deltaTime * 10);
                
        }
    }
    public void Highlight(bool highlight, bool hover)
    {
        showAsSelectable = highlight;
        showPreview = hover;
    }
}
