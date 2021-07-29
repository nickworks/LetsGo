using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesList : MonoBehaviour
{
    public GameCard PrefabGameCard;
    public RectTransform scrollContent;

    public void UpdateDisplay(GameOGS[] games) {
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

}
