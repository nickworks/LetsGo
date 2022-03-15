using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollContentMgr : MonoBehaviour
{
    public ProfileCard PrefabProfileCard;
    public GameCard PrefabGameCard;

    public RectTransform scrollContent;

    public void ShowFriends(Friend[] friends) {
        Clear();
        const float margin = 10;
        float y = - margin;
        foreach(Friend f in friends) {

            ProfileCard profileCard = Instantiate(PrefabProfileCard, scrollContent);
            profileCard.UpdateView(f);
            RectTransform rt = profileCard.transform as RectTransform;
            
            rt.anchoredPosition = new Vector2(margin, y);
            y -= (rt.sizeDelta.y + margin); // move down height + 10
        }
    }
    public void ShowMyGames(GameOGS[] games) {
        Clear();
        const float margin = 10;
        float y = -margin;
        foreach (GameOGS g in games) {

            GameCard gameCard = Instantiate(PrefabGameCard, scrollContent);
            gameCard.UpdateView(g);
            RectTransform rt = gameCard.transform as RectTransform;

            rt.anchoredPosition = new Vector2(margin, y);
            y -= (rt.sizeDelta.y + margin); // move down height + 10
        }
    }
    private void Clear(){
        for(int i = 0; i < scrollContent.childCount; i++){
            Destroy(scrollContent.GetChild(i).gameObject);
        }
    }

}
