using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleCard : MonoBehaviour, IPointerClickHandler {
    public TextMeshProUGUI textPuzzleName;
    public TextMeshProUGUI textCreatorName;
    public TextMeshProUGUI textRating;
    public TextMeshProUGUI textDifficulty;

    int puzzle_id = 0;

    public void UpdateView(ResponsePuzzleList.Puzzle g) {
        textPuzzleName.text = g.name;
        textCreatorName.text = "";
        textDifficulty.text = "";
        puzzle_id = g.id;
    }
    public string Rank(int n){
        return (n >= 30) ? $"{n-29}d" : $"{30-n}k";
    }
    public void UpdateView(ResponsePuzzleCollection.PuzzleCollection g) {
        textPuzzleName.text = g.name;
        textCreatorName.text = g.owner.username;

        if(g.min_rank == g.max_rank){
            textDifficulty.text = Rank(g.min_rank);
        } else {
            textDifficulty.text = Rank(g.min_rank)+" - "+Rank(g.max_rank);
        }

        
        textRating.text = $"{g.rating} stars ({g.rating_count} ratings)";
        puzzle_id = g.starting_puzzle.id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayController.singleton.BeginPuzzle(puzzle_id);
    }
}
