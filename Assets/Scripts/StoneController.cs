using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneController : MonoBehaviour {

    public Transform preview;
    public Transform stone;

    public int value { get; private set; }
    public bool showLargePreview { get; set; }
    public bool showPreview { get; set; }

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
        MeshRenderer mesh = stone.GetComponentInChildren<MeshRenderer>();
        mesh.material.color = val == 1 ? Color.black : Color.white;

    }
    public void SetPreviewState(int whoseTurn)
    {
        if (value > 0) return; // don't bother
        UpdateViews();
        MeshRenderer mesh = preview.GetComponentInChildren<MeshRenderer>();
        if (whoseTurn == 1) mesh.material.color = Color.black;
        if (whoseTurn == 2) mesh.material.color = Color.white;
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
            float scale = (showLargePreview) ? .08f : .005f;
            previewSize = Mathf.Lerp(previewSize, scale, Time.deltaTime * 5);
            preview.localScale = Vector3.one * previewSize;
        }
    }
}
