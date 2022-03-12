
using System.Net;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public static class RestOGS {

    #region Don't publish / commit values to repo
    private const string clientID = "";
    private const string clientSecret = "";
    #endregion

    public static OAuthToken token { get; private set; }

    public static async void Login(string username, string password) {

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("client_id", clientID);
        data.Add("client_secret", clientSecret);
        data.Add("grant_type", "password");
        data.Add("username", username);
        data.Add("password", password);

        UnityWebRequest request = UnityWebRequest.Post("https://online-go.com/oauth2/token/", data); // needs trailing slash
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        UnityWebRequestAsyncOperation task = request.SendWebRequest();

        while (!task.isDone) await System.Threading.Tasks.Task.Yield();

        switch (request.result) {
            case UnityWebRequest.Result.Success:
                //Debug.Log($"{request.downloadHandler.text}");

                token = OAuthToken.From(request.downloadHandler.text);

                break;
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError($"{request.error}");
                break;
        }
    }

    private static async Task<UnityWebRequest> Get(string command) {

        UnityWebRequest request = UnityWebRequest.Get("https://online-go.com/api/v1/"+command);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.SetRequestHeader("Authorization", $"Bearer {token.access_token}");

        UnityWebRequestAsyncOperation task = request.SendWebRequest();

        while (!task.isDone) await Task.Yield();

        return request;
    }


    public delegate void OnRestSuccess<T>(T obj);
    public delegate void OnRestFail(string error);


    private static async void Send<T>(string uri, OnRestSuccess<T> onSuccess, OnRestFail onFail) {
        if (token.expires_in == 0) return; // we lost the token

        UnityWebRequest request = await Get(uri);

        switch (request.result) {
            case UnityWebRequest.Result.Success:

                if(onSuccess != null) onSuccess(JsonConvert.DeserializeObject<T>(request.downloadHandler.text));

                break;
            case UnityWebRequest.Result.InProgress:
                // this shouldn't happen
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError($"{request.error}");
                if (onFail != null) onFail(request.error);
                break;
        }
    }

    public static void Get_MyProfile() {

        Send<ResponseMyProfile>("me", (ResponseMyProfile profile)=> {

            Debug.Log($"Your overall rating is: {profile.ratings.overall.rating}");
        
        }, (string error) => { });
    }

    public static void Get_GamesList() {

        Send<ResponseGameList>("me/games", (ResponseGameList games) => {

            GamesList gamesUI = GameObject.FindObjectOfType<GamesList>();
            if (gamesUI) gamesUI.UpdateDisplay(games.results);

        }, (string error) => { });

    }

    public static void Get_FriendsList() {

        Send<ResponseFriendsList>("me/friends", (ResponseFriendsList friends) => {

            FriendsList friendsUI = GameObject.FindObjectOfType<FriendsList>();
            if (friendsUI) friendsUI.UpdateDisplay(friends.results);


        }, (string error) => { });
    
    }
}