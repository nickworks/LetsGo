using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePanel : MonoBehaviour
{
    
    public CardList puzzleCardList;
    public PuzzleCard puzzleCard;

    void Start()
    {
        Query(0);
    }

    public void Query(int value){
        if(puzzleCard == null) return;
        if(puzzleCardList == null) return;

        RestOGS.Get_PuzzleCollectionList((Ordering)value, (puzzles) => {
            puzzleCardList.MakeCardsThen(puzzles.results, puzzleCard, (obj, data) => obj.UpdateView(data));
        });

    }
}
