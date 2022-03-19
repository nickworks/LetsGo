using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ScrollContentMgr : MonoBehaviour
{
    delegate void OnInit<TPrefab, TModel>(TPrefab obj, TModel data);
    public ProfileCard PrefabProfileCard;
    public PuzzleCard PrefabPuzzleCard;
    public GameCard PrefabGameCard;

    public RectTransform scrollContent;
    public Button undoButton;

    private void Clear(){
        for(int i = 0; i < scrollContent.childCount; i++){
            Destroy(scrollContent.GetChild(i).gameObject);
        }
    }
    public void ShowFriends(Friend[] friends) {
        Clear();
        MakeCardsThen(friends, PrefabProfileCard, (obj,data) => obj.UpdateView(data));
    }
    public void ShowMyGames(GameOGS[] games) {
        Clear();
        MakeCardsThen(games, PrefabGameCard, (obj,data) => obj.UpdateView(data));
    }
    public void ShowPuzzles(ResponsePuzzleList.Puzzle[] puzzles) {
        Clear();
        MakeCardsThen(puzzles, PrefabPuzzleCard, (obj,data) => obj.UpdateView(data));
    }
    public void ShowPuzzles(ResponsePuzzleCollection.PuzzleCollection[] puzzles) {
        Clear();
        MakeCardsThen(puzzles, PrefabPuzzleCard, (obj,data) => obj.UpdateView(data));
    }
    private void MakeCardsThen<TPrefab, TModel>(TModel[] data, TPrefab prefab, OnInit<TPrefab, TModel> callback) where TPrefab : MonoBehaviour {
        const float margin = 10;
        float y = -margin;
        foreach(TModel datum in data){
            TPrefab newCard = Instantiate(prefab, scrollContent);
            RectTransform rt = newCard.transform as RectTransform;
            rt.anchoredPosition = new Vector2(margin, y);
            y -= (rt.sizeDelta.y + margin); // move down height + 10
            callback(newCard, datum);
        }
    }
    public void Undo(){
        PlayController.singleton.PrevTurn();
        //undoButton.enabled = PlayController.singleton.HasHistory;
    }
}
