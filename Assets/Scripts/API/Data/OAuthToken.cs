using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OAuthToken
{
    public string access_token;
    public int expires_in;
    public string token_type;
    public string scope;
    public string refresh_token;

    public static OAuthToken From(string json) {
        return JsonUtility.FromJson<OAuthToken>(json);
    }
}
