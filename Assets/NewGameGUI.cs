using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameGUI : MonoBehaviour {

    public PlayController play;

    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;
    public Text labelX;
    public Text labelY;
    public Text labelZ;

    void Start () {
        UpdateLabels();	
	}
    public void UpdateLabels()
    {
        labelX.text = ""+sliderX.value;
        labelY.text = ""+sliderY.value;
        labelZ.text = ""+sliderZ.value;
    }
    public void BeginGame()
    {
        Destroy(gameObject);
        play.BeginGame((int)sliderX.value, (int)sliderY.value, (int)sliderZ.value);
    }
}
