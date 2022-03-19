using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleCard : MonoBehaviour, IPointerClickHandler {
    public TextMeshProUGUI textPuzzleName;
    public TextMeshProUGUI textCreatorName;
    public TextMeshProUGUI textDifficulty;

    int puzzle_id = 0;

    public void UpdateView(ResponsePuzzleList.Puzzle g) {
        textPuzzleName.text = g.name;
        textCreatorName.text = "";
        textDifficulty.text = "";
        puzzle_id = g.id;
    }
    public void UpdateView(ResponsePuzzleCollection.PuzzleCollection g) {
        textPuzzleName.text = g.name;
        textCreatorName.text = g.owner.username;
        textDifficulty.text = "";
        puzzle_id = g.starting_puzzle.id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayController.singleton.BeginPuzzle(puzzle_id);
    }
}
