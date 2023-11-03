using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpectateCard : MonoBehaviour {
    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textPlayerWhite;
    public TextMeshProUGUI textPlayerBlack;

    public void UpdateView(ResponseGameQuery.GameOGS g) {
        textTitle.text = g.name;
        textPlayerWhite.text = g.white.username + $" ({g.white})";
        textPlayerBlack.text = g.black.username + $" ({g.black})";
    }
}
