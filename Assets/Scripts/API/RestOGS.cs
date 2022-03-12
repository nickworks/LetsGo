
using System.Net;
using System.Threading.Tasks;
using System.Text;

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

    private static async void PostString(string uri, List<IMultipartFormSection> data = null, OnRestSuccess onSuccess = null, OnRestFail onFail = null, bool sendToken = true, string method = "POST") {
        if (sendToken && token.expires_in == 0) return; // we lost the token

        if (data == null) data = MakeData();

        byte[] boundary = UnityWebRequest.GenerateBoundary();
        UnityWebRequest request = UnityWebRequest.Post(uri, data, boundary);
        request.method = method;
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
    private static void Post<T>(string uri, List<IMultipartFormSection> data = null, OnRestSuccess<T> onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        PostString(uri, data, (string text) => {
            if (onSuccess != null) onSuccess(JsonConvert.DeserializeObject<T>(text));
        }, onFail, sendToken);
    }
    public static List<IMultipartFormSection> MakeData(){
        return new List<IMultipartFormSection>();
    }
    public static List<IMultipartFormSection> Add(this List<IMultipartFormSection> data, string key, string value){
        data.Add(new MultipartFormDataSection(key, value));
        return data;
    }
    public static List<IMultipartFormSection> Add(this List<IMultipartFormSection> data, string key, float value){
        data.Add(new MultipartFormDataSection(key, System.BitConverter.GetBytes(value)));
        return data;
    }
    public static class API {
        public static void Get_MyProfile() {
            Get<ResponseMyProfile>($"{pathAPI}me", (profile) => {

                Debug.Log($"Your overall rating is: {profile.ratings.overall.rating}");

            }, (string error) => { });
        }
        public static void Get_GamesList() {
            Get<ResponseGameList>($"{pathAPI}me/games", (games) => {

                GamesList gamesUI = GameObject.FindObjectOfType<GamesList>();
                if (gamesUI) gamesUI.UpdateDisplay(games.results);

            }, (string error) => { });
        }
        public static void Get_FriendsList() {
            Get<ResponseFriendsList>($"{pathAPI}me/friends", (friends) => {

                FriendsList friendsUI = GameObject.FindObjectOfType<FriendsList>();
                if (friendsUI) friendsUI.UpdateDisplay(friends.results);


            }, (string error) => { });
        }
        public static void Get_PuzzleList() {
            Get<ResponsePuzzleList>($"{pathAPI}puzzles", (puzzles) => {

                StringBuilder sb = new StringBuilder();
                foreach (var puzzle in puzzles.results) {
                    sb.AppendLine(puzzle.name);
                }
                Debug.Log(sb.ToString());

            }, (string error) => { });
        }
        public static void Get_ChallengeList() {

            
        }
        public static void Post_Login(string username, string password) {

            List<IMultipartFormSection> data = MakeData();
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