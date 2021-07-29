using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FriendsList : MonoBehaviour
{
    public ProfileCard PrefabProfileCard;

    public RectTransform scrollContent;

    public void UpdateDisplay(Friend[] friends) {

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

}
