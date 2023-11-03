using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneController : MonoBehaviour {

    public Transform preview;
    public Transform stone;

    private MeshRenderer meshStone;
    private MeshRenderer meshPreview;

    public int value { get; private set; }
    public bool showLargePreview { get; private set; }

    List<StoneController> neighbors = new List<StoneController>();

    public void AddNeighbor(StoneController stone)
    {
        if(!neighbors.Contains(stone)) neighbors.Add(stone);
    }
    public void SetPlayer(bool isPlayer1, bool isVisible = true)
    {
        int val = 0;
        if (isVisible) val = isPlayer1 ? 1 : 2;
        SetGameState(val);
    }
    public void SetGameState(int val)
    {
        value = val;
        UpdateViews();
        if(!meshStone) meshStone = stone.GetComponentInChildren<MeshRenderer>();
        meshStone.material.color = val == 1 ? Color.black : Color.white;
        showLargePreview = false;
        if (val > 0) previewSize = 0;
    }
    public void SetPreviewShow(bool showPreview) {
        showLargePreview = value > 0 ? false : showPreview;
    }
    public void SetPreviewState(int whoseTurn)
    {
        if (value > 0) return;
        UpdateViews();
        if(!meshPreview) meshPreview = preview.GetComponentInChildren<MeshRenderer>();
        if (whoseTurn == 1) meshPreview.material.color = Color.black;
        if (whoseTurn == 2) meshPreview.material.color = Color.white;
    }
    void UpdateViews()
    {
        stone.gameObject.SetActive(value>0);
        preview.gameObject.SetActive(value==0);
    }


    float previewSize = 0;
    void Update()
    {
        if (preview.gameObject.activeSelf)
        {
            float scale = (showLargePreview) ? .08f : .00f;
            previewSize = Mathf.Lerp(previewSize, scale, Time.deltaTime * 5);
            preview.localScale = Vector3.one * previewSize;
        }
    }

}
