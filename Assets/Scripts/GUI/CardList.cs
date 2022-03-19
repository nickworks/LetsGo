using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardList : MonoBehaviour
{
    public delegate void OnInit<TPrefab, TModel>(TPrefab obj, TModel data);
    private void Clear(){
        for(int i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void MakeCardsThen<TPrefab, TModel>(TModel[] data, TPrefab prefab, OnInit<TPrefab, TModel> callback) where TPrefab : MonoBehaviour {
        Clear();
        const float margin = 10;
        float y = -margin;
        foreach(TModel datum in data){
            TPrefab newCard = Instantiate(prefab, transform);
            RectTransform rt = newCard.transform as RectTransform;
            rt.anchoredPosition = new Vector2(margin, y);
            y -= (rt.sizeDelta.y + margin); // move down height + 10
            callback(newCard, datum);
        }
    }
}
