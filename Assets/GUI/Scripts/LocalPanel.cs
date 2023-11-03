using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalPanel : MonoBehaviour { 
    public TMP_Text labelX;
    public TMP_Text labelY;
    public Slider sliderX;
    public Slider sliderY;
    public Transform panelCustomSize;
    
    void Start() {
        sliderX?.onValueChanged.AddListener(n=>SlidersChangeSize());
        sliderY?.onValueChanged.AddListener(n=>SlidersChangeSize());
    }
    void SlidersChangeSize(){
        if(sliderX && labelX) labelX.text = sliderX.value.ToString();
        if(sliderY && labelY) labelY.text = sliderY.value.ToString();
    }
    public void StartGame9x9(){
        PlayController.singleton.BeginGame(9,9,1);
    }
    public void StartGame13x13(){
        PlayController.singleton.BeginGame(13,13,1);
    }
    public void StartGame19x19(){
        PlayController.singleton.BeginGame(19,19,1);
    }
    public void ToggleCustomSize(){
        panelCustomSize.gameObject.SetActive(panelCustomSize.gameObject.activeSelf);
    }
    public void ShowCustomSize(){
        panelCustomSize.gameObject.SetActive(true);
    }
    public void HideCustomSize(){
        panelCustomSize.gameObject.SetActive(false);
    }
    public void StartGameCustom(){
        int x = (int)sliderX.value;
        int y = (int)sliderY.value;
        PlayController.singleton.BeginGame(x,y,1);
    }
}
