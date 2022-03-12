
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

    private const string pathAPI = @"https://online-go.com/api/v1/";
    private const string pathAuth = @"https://online-go.com/oauth2/token/";

    private static OAuthToken token;


    public delegate void OnRestSuccess<T>(T obj);
    public delegate void OnRestSuccess(string text);
    public delegate void OnRestFail(string error);

    private static async void PostString(string uri, Dictionary<string, string> data = null, OnRestSuccess onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        if (sendToken && token.expires_in == 0) return; // we lost the token

        if (data == null) data = new Dictionary<string, string>();

        UnityWebRequest request = UnityWebRequest.Post(uri, data); // needs trailing slash
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        if(sendToken) request.SetRequestHeader("Authorization", $"Bearer {token.access_token}");
        UnityWebRequestAsyncOperation task = request.SendWebRequest();

        while (!task.isDone) await Task.Yield();

        switch (request.result) {
            case UnityWebRequest.Result.Success:

                if (onSuccess != null) onSuccess(request.downloadHandler.text);

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
    private static async void GetString(string uri, OnRestSuccess onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        if (sendToken && token.expires_in == 0) return; // we lost the token

        UnityWebRequest request = UnityWebRequest.Get(uri);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        if (sendToken) request.SetRequestHeader("Authorization", $"Bearer {token.access_token}");
        UnityWebRequestAsyncOperation task = request.SendWebRequest();

        while (!task.isDone) await Task.Yield();

        switch (request.result) {
            case UnityWebRequest.Result.Success:

                if (onSuccess != null) onSuccess(request.downloadHandler.text);

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
    private static void Get<T>(string uri, OnRestSuccess<T> onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        GetString(uri, (string text) => {
            if (onSuccess != null) onSuccess(JsonConvert.DeserializeObject<T>(text));
        }, onFail, sendToken);
    }
    private static void Post<T>(string uri, Dictionary<string, string> data = null, OnRestSuccess<T> onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        PostString(uri, data, (string text) => {
            if (onSuccess != null) onSuccess(JsonConvert.DeserializeObject<T>(text));
        }, onFail, sendToken);
    }
    public static class API {
        public static void Get_MyProfile() {

            Get<ResponseMyProfile>($"{pathAPI}me", (ResponseMyProfile profile) => {

                Debug.Log($"Your overall rating is: {profile.ratings.overall.rating}");

            }, (string error) => { });
        }

        public static void Get_GamesList() {

            Get<ResponseGameList>($"{pathAPI}me/games", (ResponseGameList games) => {

                GamesList gamesUI = GameObject.FindObjectOfType<GamesList>();
                if (gamesUI) gamesUI.UpdateDisplay(games.results);

            }, (string error) => { });

        }

        public static void Get_FriendsList() {

            Get<ResponseFriendsList>($"{pathAPI}me/friends", (ResponseFriendsList friends) => {

                FriendsList friendsUI = GameObject.FindObjectOfType<FriendsList>();
                if (friendsUI) friendsUI.UpdateDisplay(friends.results);


            }, (string error) => { });

        }
        public static void Post_Login(string username, string password) {

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("client_id", clientID);
            data.Add("client_secret", clientSecret);
            data.Add("grant_type", "password");
            data.Add("username", username);
            data.Add("password", password);

            PostString(pathAuth, data, (string text) => {
                token = OAuthToken.From(text);
            }, (string error) => { }, false);

        }
    }
}