using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json;

public class SocketOGS {
    
    public SocketIOUnity socket { get; private set;}
    public SocketOGS(){
        
    }
    public void Connect() {

        if(socket != null) return; // already connected/ing or failed

        System.Uri uri = new System.Uri("https://online-go.com/");

        SocketIOOptions options = new SocketIOOptions{
            Query = new Dictionary<string, string>{ {"token", "UNITY" } },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            ReconnectionAttempts = 2,
        };

        socket = new SocketIOUnity(uri, options);
        socket.JsonSerializer = new NewtonsoftJsonSerializer();


        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };
        socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{System.DateTime.Now} Reconnecting: attempt = {e}");
        };

        socket.OnAnyInUnityThread((name, response) =>
        {
            string text = response.GetValue().GetRawText();
            Debug.Log("[" + name + "] : " + text);

            switch(name){
                case "active-bots":

                    // this response has the bots' ids as json keys
                    // so we have to deserialize the response this way:
                    Dictionary<int, ResponseActiveBots> bots =
                        JsonConvert.DeserializeObject<Dictionary<int, ResponseActiveBots>>(text);
                    
                    // loop through the bots list:
                    foreach(KeyValuePair<int, ResponseActiveBots> bot in bots){
                        //Debug.Log(bot.Value.username);
                    }
                    
                break;
            }
        });
        socket.Connect();
        
    }
    public async void Disconnect(){
        if(socket == null) return;

        socket.Disconnect();
        await socket.DisconnectAsync();
        socket.Dispose();
        
        socket = null;
    }
}
