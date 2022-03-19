using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NetController : MonoBehaviour
{

    public string user = "test-user";
    public string pass = "#dev#";

    [TextArea(1,3)]
    public string json = "";

    public SocketOGS socket { get; private set; }
    void Start(){
        socket = new SocketOGS();
    }
    void OnDestroy(){
        socket.Disconnect();
        socket = null;
    }
}

[CustomEditor(typeof(NetController))]
public class NetControllerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        NetController rt = (target as NetController);

        GUILayout.Space(15);
        GUILayout.Label("REST API");
        GUILayout.Space(10);

        if (GUILayout.Button("Login")) RestOGS.Login(rt.user, rt.pass, (s)=>{});

        if (GUILayout.Button("List My Friends")) RestOGS.Get_FriendsList((friends) => { });
        if (GUILayout.Button("List My Games")) RestOGS.Get_GamesList((games) => { });
        if (GUILayout.Button("Get My Profile")) RestOGS.Get_MyProfile(profile => { });
        
        if (GUILayout.Button("List Puzzles")) RestOGS.Get_PuzzleCollectionList(Ordering.HighestRating, (puzzles) => {
            //ScrollContentMgr uiScrollContent = GameObject.FindObjectOfType<ScrollContentMgr>();
            //if (uiScrollContent) uiScrollContent.ShowPuzzles(puzzles.results);

        });
        
        if (GUILayout.Button("Load JSON puzzle")) PlayController.singleton.BeginPuzzle(rt.json);
        if (GUILayout.Button("List JSON Puzzles")) {

            var games = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponsePuzzleCollection>(rt.json);

            //ScrollContentMgr uiScrollContent = GameObject.FindObjectOfType<ScrollContentMgr>();
            //if (uiScrollContent) uiScrollContent.ShowPuzzles(games.results);
        }

        GUILayout.Space(15);
        GUILayout.Label("Real-time API");
        GUILayout.Space(10);
        if (GUILayout.Button("Connect Socket")) rt.socket.Connect();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start SeekGraph")) rt.socket.StartSeekGraph();
        if (GUILayout.Button("Stop SeekGraph")) rt.socket.StopSeekGraph();
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Fetch Live Games")) rt.socket.FetchGames();
        
    }
}
