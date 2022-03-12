using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Http;
using System.IO;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

public class RestTester : MonoBehaviour
{

    public string user = "test-user";
    public string pass = "#dev#";

}

[CustomEditor(typeof(RestTester))]
public class RestTesterEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        RestTester rt = (target as RestTester);

        if (GUILayout.Button("Login"))              RestOGS.API.Post_Login(rt.user, rt.pass);
        if (GUILayout.Button("List My Friends"))    RestOGS.API.Get_FriendsList();
        if (GUILayout.Button("List My Games"))      RestOGS.API.Get_GamesList();
        if (GUILayout.Button("Get My Profile"))     RestOGS.API.Get_MyProfile();
        if (GUILayout.Button("List Puzzles"))       RestOGS.API.Get_PuzzleList();

    }
}
