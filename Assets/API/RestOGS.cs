
using System.Net;
using System.Threading.Tasks;
using System.Text;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public enum Ordering {
    HighestRating,
    LowestDifficulty,
    HighestDifficulty,
    Newest,
    Oldest
}
public static class RestOGS {

    #region Don't publish / commit values to repo
    private const string clientID = @"";
    private const string clientSecret = @"";
    #endregion

    #region Delegate Declarations
    public delegate void OnRestSuccess<T>(T obj);
    public delegate void OnRestSuccess(string text);
    public delegate void OnRestFail(string error);
    #endregion
    
    private static OAuthToken token;

    #region OGS Endpoints
    private const string pathAPI = @"https://online-go.com/api/v1/";
    private const string pathAuth = @"https://online-go.com/oauth2/token/";
    
    public static void Login(string username, string password, OnRestSuccess onSuccess, OnRestFail onFail = null) {

        List<IMultipartFormSection> data = MakePostData();
        data.Add("client_id", clientID);
        data.Add("client_secret", clientSecret);
        data.Add("grant_type", "password");
        data.Add("username", username);
        data.Add("password", password);

        Post<OAuthToken>(pathAuth, data, (t) => {
            token = t;
            Debug.Log(token);

            onSuccess("");

        }, onFail, false);
    }
    
    public static void Get_MyProfile(OnRestSuccess<ResponseMyProfile> onSuccess, OnRestFail onFail = null) {
        Get<ResponseMyProfile>($"{pathAPI}me", onSuccess, onFail);
    }
    public static void Get_GamesList(OnRestSuccess<ResponseGameList> onSuccess, OnRestFail onFail = null) {
        Get<ResponseGameList>($"{pathAPI}me/games", onSuccess, onFail);
    }
    public static void Get_FriendsList(OnRestSuccess<ResponseFriendsList> onSuccess, OnRestFail onFail = null) {
        Get<ResponseFriendsList>($"{pathAPI}me/friends", onSuccess, onFail);
    }
    public static void Get_PuzzleList(OnRestSuccess<ResponsePuzzleList> onSuccess, OnRestFail onFail = null) {
        Get<ResponsePuzzleList>($"{pathAPI}puzzles", onSuccess, onFail);
    }
    public static void Get_PuzzleCollectionList(Ordering order, OnRestSuccess<ResponsePuzzleCollection> onSuccess, OnRestFail onFail = null) {

        string ordering = "-rating";
        if(order == Ordering.HighestDifficulty) ordering = "-min_rank,-max_rank";
        if(order == Ordering.LowestDifficulty) ordering = "min_rank,max_rank";
        if(order == Ordering.Newest) ordering = "-created";
        if(order == Ordering.Oldest) ordering = "+created";

        Get<ResponsePuzzleCollection>($"{pathAPI}puzzles/collections?ordering={ordering}", onSuccess, (string error) => { });
    }
    // this loads a specific puzzle
    public static void Get_Puzzle(int puzzle_id, OnRestSuccess<ResponsePuzzle> onSuccess, OnRestFail onFail = null){
        Get<ResponsePuzzle>($"{pathAPI}puzzles/{puzzle_id}", onSuccess, onFail);
    }
    // this loads the id / name of each puzzle in a collection
    public static void Get_PuzzleCollectionSummary(int puzzle_id, OnRestSuccess<ResponsePuzzleSummary.Puzzle[]> onSuccess, OnRestFail onFail = null){
        Get<ResponsePuzzleSummary.Puzzle[]>($"{pathAPI}puzzles/{puzzle_id}/collection_summary", onSuccess, onFail);
    }
    public static void Get_PuzzleRate(int puzzle_id, OnRestSuccess<ResponsePuzzleRate> onSuccess, OnRestFail onFail = null){
        Get<ResponsePuzzleRate>($"{pathAPI}puzzles/{puzzle_id}/rate", onSuccess, onFail);
    }
    
    #endregion
    #region Internal Stuff

    // analyzes the result of a web request
    // and calls either onSuccess or onFail
    private static void RouteResponse(UnityWebRequest request, OnRestSuccess onSuccess, OnRestFail onFail) {
        switch (request.result) {
            case UnityWebRequest.Result.Success:
                if (onSuccess != null) onSuccess(request.downloadHandler.text);
                break;
            case UnityWebRequest.Result.InProgress:
                // this shouldn't happen?
                Debug.Log("waiting... [REST in progress]");
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError($"{request.error}");
                if (onFail != null) onFail(request.error);
                break;
        }
    }

    // this builds a POST request, sends it,
    // awaits for a response, and then triggers
    // either onSuccess or onFail
    private static async void PostString(string uri, List<IMultipartFormSection> data = null, OnRestSuccess onSuccess = null, OnRestFail onFail = null, bool sendToken = true, string method = "POST")
    {
        if (sendToken && token.expires_in == 0) return; // we lost the token

        if (data == null) data = MakePostData();

        byte[] boundary = UnityWebRequest.GenerateBoundary();
        UnityWebRequest request = UnityWebRequest.Post(uri, data, boundary);
        request.method = method;
        if (sendToken) request.SetRequestHeader("Authorization", $"Bearer {token.access_token}");

        UnityWebRequestAsyncOperation task = request.SendWebRequest();
        while (!task.isDone) await Task.Yield();
        RouteResponse(request, onSuccess, onFail);
    }

    // this builds a GET request, sends it,
    // awaits for a response, and then triggers
    // either onSuccess or onFail
    private static async void GetString(string uri, OnRestSuccess onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        if (sendToken && token != null && token.expires_in == 0) return; // we lost the token

        UnityWebRequest request = UnityWebRequest.Get(uri);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        if (sendToken && token != null) request.SetRequestHeader("Authorization", $"Bearer {token.access_token}");
        UnityWebRequestAsyncOperation task = request.SendWebRequest();

        while (!task.isDone) await Task.Yield();
        RouteResponse(request, onSuccess, onFail);
    }
    // deserializes a GET response into an object
    private static void Get<T>(string uri, OnRestSuccess<T> onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        GetString(uri, (string text) => {
            if (onSuccess != null) {
                Debug.Log(text);
                T obj = JsonConvert.DeserializeObject<T>(text);
                onSuccess(obj);
            }
        }, onFail, sendToken);
    }
    // deserializes a POST response into an object
    private static void Post<T>(string uri, List<IMultipartFormSection> data = null, OnRestSuccess<T> onSuccess = null, OnRestFail onFail = null, bool sendToken = true) {
        PostString(uri, data, (string text) => {
            if (onSuccess != null) {
                T obj = JsonConvert.DeserializeObject<T>(text);
                onSuccess(obj);
            }
        }, onFail, sendToken);
    }
    private static List<IMultipartFormSection> MakePostData(){
        return new List<IMultipartFormSection>();
    }
    #endregion
    #region Extension Functions
    // extends list to allow for easier key / pair additions (strings)
    public static List<IMultipartFormSection> Add(this List<IMultipartFormSection> data, string key, string value){
        data.Add(new MultipartFormDataSection(key, value));
        return data;
    }
    // extends list to allow for easier key / pair additions (floats)
    public static List<IMultipartFormSection> Add(this List<IMultipartFormSection> data, string key, float value){
        data.Add(new MultipartFormDataSection(key, System.BitConverter.GetBytes(value)));
        return data;
    }
    #endregion
}