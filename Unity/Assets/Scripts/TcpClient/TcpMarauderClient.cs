using NetCoreServer;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class TcpMarauderClient : MonoBehaviour
{

    private string serverIp;

    private int serverPort;

    // Try to reconnect if connection could not be established or was lost
    private bool reconnect = true;
    private float reconnectionDelay = 1.0f;

    // Used implementation as interface to allow easy switching
    private IUnitySocketClient socketClient;

    // Buffer will be used for sending and receiving to avoid creating garbage
    private byte[] buffer;
    private bool disconnecting = false;
    private bool applicationQuitting;

    public event NotifyControllerDelegate NotifyParentEvent;

    private string s = "";

    void Start()
    {
        disconnecting = false;
    }


    public string GetId()
    {
        return socketClient.Id.ToString();
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    [ContextMenu("Connect")]
    private void Connect()
    {
        if (socketClient != null && (socketClient.IsConnected || socketClient.IsConnecting))
        {
            Disconnect();
            socketClient = null;
        }

        socketClient = new UnityTcpClient(serverIp, serverPort);

        buffer = new byte[socketClient.OptionReceiveBufferSize];

        socketClient.OnConnectedEvent += OnConnected;
        socketClient.OnDisconnectedEvent += OnDisconnected;
        socketClient.OnErrorEvent += OnError;
        socketClient.ConnectAsync();
    }

    [ContextMenu("Disconnect")]

    public void Disconnect()
    {
        if (socketClient == null)
        {
            return;
        }

        disconnecting = true;
        socketClient.Disconnect();

        while (socketClient.IsConnected)
        {
            Thread.Yield();
        }

        socketClient.OnConnectedEvent -= OnConnected;
        socketClient.OnDisconnectedEvent -= OnDisconnected;
        socketClient.OnErrorEvent -= OnError;
        disconnecting = false;
    }

    void Update()
    {
        bool connected = socketClient != null && socketClient.IsConnected;

        if (!connected || !socketClient.HasEnqueuedPackages())
        {
            return;
        }

        while (socketClient.HasEnqueuedPackages())
        {
            s = "";
            int length = socketClient.GetNextPackage(ref buffer);
            var message = Encoding.UTF8.GetString(buffer, 0, length);
            s += message + "\n";
            NotifyParentEvent?.Invoke();
        }
    }

    public string GetS()
    {
        return s;
    }

    private void OnConnected()
    {
        Debug.Log($"{socketClient.GetType()} connected a new session with Id {socketClient.Id}");
    }

    private void OnDisconnected()
    {
        Debug.Log($"{socketClient.GetType()} disconnected a session with Id {socketClient.Id}");
        if (applicationQuitting)
        {
            return;
        }

        if (reconnect && !disconnecting)
        {
            ReconnectDelayedAsync();
        }
    }

    private async void ReconnectDelayedAsync()
    {
        await Task.Delay((int)(reconnectionDelay * 1000));

        if (socketClient.IsConnected || socketClient.IsConnecting)
        {
            return;
        }

        Debug.Log("Trying to reconnect");
        socketClient.ConnectAsync();
    }

    private void OnError(SocketError error)
    {
        Debug.LogError($"{socketClient.GetType()} caught an error with code {error}");
    }

    private void TriggerDisconnect()
    {
        Disconnect();
    }

    public void ApplyInputAndConnect(string serverIpInput, int serverPortInput)
    {
        serverIp = serverIpInput;
        serverPort = serverPortInput;
        Connect();
    }

    public void SendEcho(byte[] message)
    {
        socketClient.Send(message);
    }

    public void SendAsynch(byte[] message)
    {
        socketClient.SendAsync(message);
    }

    private void OnApplicationQuit()
    {
        applicationQuitting = true;
    }
}
