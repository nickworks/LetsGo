using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameGUI : MonoBehaviour {

    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;
    public Text labelX;
    public Text labelY;
    public Text labelZ;

    void Start () {
        UpdateLabels();	
	}
    public void LoadMeshExample()
    {
        SceneManager.LoadScene("MeshTest");
    }
    public void UpdateLabels()
    {
        labelX.text = ""+sliderX.value;
        labelY.text = ""+sliderY.value;
        labelZ.text = ""+sliderZ.value;
    }
    public void BeginGame()
    {
        PlayController.sizex = (int)sliderX.value;
        PlayController.sizey = (int)sliderY.value;
        PlayController.sizez = (int)sliderZ.value;
        SceneManager.LoadScene("Goban");
    }
}
