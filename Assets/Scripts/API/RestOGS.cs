
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

    public static async void Get_MyProfile() {

        if (token.expires_in == 0) return; // we lost the token

        UnityWebRequest request = await Get("me");

        switch (request.result) {
            case UnityWebRequest.Result.Success:

                ResponseMyProfile myProfile = JsonConvert.DeserializeObject<ResponseMyProfile>(request.downloadHandler.text);
                
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

    public static async void Get_GamesList() {

        if (token.expires_in == 0) return; // we lost the token

        UnityWebRequest request = await Get("me/games/");

        switch (request.result) {
            case UnityWebRequest.Result.Success:

                ResponseGameList games = JsonConvert.DeserializeObject<ResponseGameList>(request.downloadHandler.text);

                GamesList gamesUI = GameObject.FindObjectOfType<GamesList>();

                if (gamesUI) gamesUI.UpdateDisplay(games.results);

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

    public static async void Get_FriendsList() {

        if (token.expires_in == 0) return; // we lost the token

        UnityWebRequest request = await Get("me/friends/");

        switch (request.result) {
            case UnityWebRequest.Result.Success:

                ResponseFriendsList friends = JsonConvert.DeserializeObject<ResponseFriendsList>(request.downloadHandler.text);

                FriendsList friendsUI = GameObject.FindObjectOfType<FriendsList>();
                if (friendsUI) friendsUI.UpdateDisplay(friends.results);

                break;
            case UnityWebRequest.Result.InProgress:
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(request.error);
                break;
        }
    }

}