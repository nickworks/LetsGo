using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ProfileCard : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;

    public void UpdateView(Friend f)
    {

        text.text = f.username;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(f.icon);

        UnityWebRequestAsyncOperation task = request.SendWebRequest();
        task.completed += (AsyncOperation obj) => {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            
            image.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height), new Vector2(0.5f, 0.5f));
        };
    }
}
