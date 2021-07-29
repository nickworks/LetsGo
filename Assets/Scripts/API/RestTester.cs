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

        if (GUILayout.Button("Login"))          RestOGS.Login(rt.user, rt.pass);
        if (GUILayout.Button("List Friends"))   RestOGS.UpdateFriendsList();
        if (GUILayout.Button("List Games"))     RestOGS.UpdateGamesList();

    }
}
