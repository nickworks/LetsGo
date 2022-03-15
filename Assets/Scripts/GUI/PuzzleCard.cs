using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleCard : MonoBehaviour {
    public TextMeshProUGUI textPuzzleName;
    public TextMeshProUGUI textCreatorName;
    public TextMeshProUGUI textDifficulty;

    public void UpdateView(ResponsePuzzleList.Puzzle g) {
        textPuzzleName.text = g.name;
        textCreatorName.text = "";
        textDifficulty.text = "";
    }
    public void UpdateView(ResponsePuzzleCollection.PuzzleCollection g) {
        textPuzzleName.text = g.name;
        textCreatorName.text = g.owner.username;
        textDifficulty.text = "";
    }
}
