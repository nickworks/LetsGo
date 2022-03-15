using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NetController : MonoBehaviour
{

    public string user = "test-user";
    public string pass = "#dev#";

    public SocketOGS socket { get; private set; }
    void Start(){
        socket = new SocketOGS();
    }
    public void ConnectSocket(){
        socket.Connect();
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
        if (GUILayout.Button("Login"))              RestOGS.API.Post_Login(rt.user, rt.pass);
        if (GUILayout.Button("List My Friends"))    RestOGS.API.Get_FriendsList();
        if (GUILayout.Button("List My Games"))      RestOGS.API.Get_GamesList();
        if (GUILayout.Button("Get My Profile"))     RestOGS.API.Get_MyProfile();
        if (GUILayout.Button("List Puzzles"))       RestOGS.API.Get_PuzzleList();
        GUILayout.Space(15);
        GUILayout.Label("Real-time API");
        GUILayout.Space(10);
        if (GUILayout.Button("Connect Socket"))     rt.ConnectSocket();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start SeekGraph"))    rt.socket.socket.Emit("seek_graph/connect", new {channel="global"});
        if (GUILayout.Button("Stop SeekGraph"))    rt.socket.socket.Emit("seek_graph/disconnect", new {channel="global"});
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Fetch Live Games"))    rt.socket.socket.EmitAsync("gamelist/query", (response)=>{
            Debug.Log(response.GetValue().ToString());
        },new{
            list="live",
            sort_by="rank",
            from=0,
            limit=5
        });
        
    }
}
