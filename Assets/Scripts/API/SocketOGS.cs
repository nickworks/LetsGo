using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;

public class SocketOGS {
    
    public SocketIOUnity socket { get; private set;}
    public SocketOGS(){
        System.Uri uri = new System.Uri("https://online-go.com/");

        SocketIOOptions options = new SocketIOOptions{
            Query = new Dictionary<string, string>{ {"token", "UNITY" } },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
            ReconnectionAttempts = 2,
        };

        socket = new SocketIOUnity(uri, options);
        socket.JsonSerializer = new NewtonsoftJsonSerializer();
    }
    public void Connect() {

        if(socket.Connected) return; // already connected

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

        socket.Connect();

        socket.OnAnyInUnityThread((name, response) =>
        {
            Debug.Log("[" + name + "] : " + response.GetValue().GetRawText());
        });
    }
    public async void Disconnect(){
        socket.Disconnect();
        await socket.DisconnectAsync();
        socket.Dispose();
        socket = null;
    }
}
