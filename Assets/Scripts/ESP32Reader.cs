using System;
using System.Text;
using UnityEngine;
using TMPro;
using NativeWebSocket;

public class ESP32WebSocketReader : MonoBehaviour
{
    public string websocketUrl = "ws://192.168.1.100:81";
    public TMP_Text sensorText;

    private WebSocket websocket;

    [Serializable]
    public class SensorData
    {
        public int analog;
        public int millivolts;
    }

    async void Start()
    {
        websocket = new WebSocket(websocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket connected");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket error: " + e);
            if (sensorText != null) sensorText.text = "WebSocket error";
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed");
            if (sensorText != null) sensorText.text = "WebSocket closed";
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log(message);

            SensorData data = JsonUtility.FromJson<SensorData>(message);
            if (sensorText != null)
            {
                sensorText.text = "Analog: " + data.analog + "\nMillivolts: " + data.millivolts;
            }
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }
}