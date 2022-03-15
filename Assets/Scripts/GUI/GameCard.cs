using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCard : MonoBehaviour {
    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textPlayerWhite;
    public TextMeshProUGUI textPlayerBlack;

    public void UpdateView(GameOGS g) {
        textTitle.text = g.name;
        textPlayerWhite.text = g.players.white.username + $" ({g.white})";
        textPlayerBlack.text = g.players.black.username + $" ({g.black})";
    }
}
