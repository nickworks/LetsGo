using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePanel : MonoBehaviour {
    
    public CardList puzzleCardList;
    public PuzzleCard puzzleCard;

    void Start() {
        Query();
    }

    public void Query(Ordering value = Ordering.HighestRating){
        if(puzzleCard == null) return;
        if(puzzleCardList == null) return;

        RestOGS.Get_PuzzleCollectionList(value, (puzzles) => {
            puzzleCardList.MakeCardsThen(puzzles.results, puzzleCard, (obj, data) => obj.UpdateView(data));
        });
    }
}
