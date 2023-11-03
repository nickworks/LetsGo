using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

    static public MenuController singleton {get; private set;}

    [System.Serializable]
    public class MenuPanel {
        public UnityEngine.UI.Toggle menuButton;
        public RectTransform panel;
    }
    public MenuPanel[] panels;
    void Start () {
        if(singleton != null){
            Destroy(gameObject);
            return;
        }
        singleton = this;

        // hide / show panels when buttons are pressed:
        foreach(MenuPanel p in panels){
            p.menuButton.onValueChanged.AddListener((on)=>{
                if(!on) return; // if this button was just turned off, don't do anything
                if(!p.panel || !p.panel.gameObject.activeSelf) HideAll(); // hide other panels (if switching)
                if(p.panel) p.panel.gameObject.SetActive(true); // show this panel
            });
        }
	}
    void OnDestroy(){
        if(singleton == this){
            singleton = null;
        }
    }
    public void HideAll(){
        foreach(MenuPanel p in panels){
            if(p.panel) p.panel.gameObject.SetActive(false);
        }
    }
    void Update(){
        HandleTabNavigation();
    }
    void HandleTabNavigation(){
        if (Input.GetKeyDown(KeyCode.Tab)) {
            EventSystem system = EventSystem.current;
            GameObject curObj = system.currentSelectedGameObject;
            GameObject nextObj = null;
            if (!curObj)  nextObj = system.firstSelectedGameObject;
            else {
                Selectable curSelect = curObj.GetComponent<Selectable>();
                Selectable nextSelect =
                    Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
                        ? curSelect.FindSelectableOnUp()
                        : curSelect.FindSelectableOnDown();

                if (nextSelect) nextObj = nextSelect.gameObject;
            }
            if (nextObj) system.SetSelectedGameObject(nextObj, new BaseEventData(system));
        }
    }
    
}
