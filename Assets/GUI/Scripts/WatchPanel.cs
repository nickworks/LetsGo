using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchPanel : MonoBehaviour
{
    //public ResponseGameQuery games;
    public CardList spectateCardList;
    public SpectateCard prefabCard;

    public string weird = "bananas";

    SocketOGS socket;
    bool initialRequest = false;
    void Start()
    {
        socket = new SocketOGS();
    }
    void Update(){
        if(socket.isConnected){
            if(!initialRequest){
                initialRequest = true;
                RequestGames();
            }
        }
    }
    void RequestGames(){
        Debug.Log("???0");
        socket.FetchGames(this);
    }
    public void ShowGames(ResponseGameQuery games){
        Debug.Log("Resonse received!");
        if(games == null) Debug.Log("response is null?");
        else spectateCardList.MakeCardsThen(games.results, prefabCard, (o,d) => {
            o.UpdateView(d);
            print("bind!");
        });
    }
    void OnDestroy(){
        if(socket != null) socket.Disconnect();
    }
}
