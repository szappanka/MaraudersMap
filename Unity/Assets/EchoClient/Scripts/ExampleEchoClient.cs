using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetCoreServer;
using UnityEngine;

namespace Supyrb
{
	public class ExampleEchoClient : MonoBehaviour
	{
		public enum Type
		{
			Tcp
		}

		#region SettingFields

		[SerializeField]
		private ExampleEchoClientUi ui = null;

		[SerializeField]
		private string serverIp = "127.0.0.1";

		[SerializeField]
		private int serverPort = 1111;

		[Tooltip("Non-blocking async sending. Not recommended for UDP (those are already non-blocking)")]
		[SerializeField]
		private bool async = true;

		[SerializeField]
		private Type type = Type.Tcp;

		[Tooltip("Try to reconnect if connection could not be established or was lost")]
		[SerializeField]
		private bool reconnect = true;

		[SerializeField]
		private float reconnectionDelay = 1.0f;

		#endregion

		public bool Async => async;

		// Used implementation as interface to allow easy switching
		private IUnitySocketClient socketClient;

		// Buffer will be used for sending and receiving to avoid creating garbage
		private byte[] buffer;
		private bool disconnecting;
		private bool applicationQuitting;
		private void Start()
		{
			disconnecting = false;
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

		private void Update()
		{
			ui.UpdateState(socketClient);
			bool connected = socketClient != null && socketClient.IsConnected;

			if (!connected || !socketClient.HasEnqueuedPackages())
			{
				return;
			}

			string messages = $"Messages received at frame {Time.frameCount}:\n";
			while (socketClient.HasEnqueuedPackages())
			{
				int length = socketClient.GetNextPackage(ref buffer);
				var message = Encoding.UTF8.GetString(buffer, 0, length);
				messages += message + "\n";
			}

			ui.AddResponseText(messages);
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

		public void ApplyInputAndConnect(string serverIpInput, int serverPortInput, Type typeInput)
		{
			serverIp = serverIpInput;
			serverPort = serverPortInput;
			type = typeInput;
			Connect();
		}

		public void SendEcho(byte[] message)
		{
			socketClient.Send(message);
		}

		private void OnApplicationQuit()
		{
			applicationQuitting = true;
		}
	}
}
