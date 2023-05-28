// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleEchoClientUi.cs">
//   Copyright (c) 2020 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using NetCoreServer;
using UnityEngine;
using UnityEngine.UI;

namespace Supyrb
{
	public class ExampleEchoClientUi : MonoBehaviour
	{
		[SerializeField] private ExampleEchoClient client = null;

		#region UiFields

		[SerializeField] private Button tcpConnectButton = null;

		[SerializeField] private Button disconnectButton = null;

		[Header("Connection")] [SerializeField]
		private InputField messageInputField = null;

		[SerializeField] private Button sendMessageButton = null;

		[SerializeField] private Text serverResponseText = null;

		[SerializeField] private Text stateInfoText = null;

		#endregion

		private void Start()
		{
			tcpConnectButton.onClick.AddListener(TriggerTcpConnect);
			disconnectButton.onClick.AddListener(TriggerDisconnect);
			sendMessageButton.onClick.AddListener(OnSendEcho);
		}

		public void UpdateState(IUnitySocketClient socketClient)
		{
			UpdateStateInfoText(socketClient);
			bool connected = socketClient != null && socketClient.IsConnected;
			sendMessageButton.interactable = connected;
			disconnectButton.interactable = connected;
			tcpConnectButton.interactable = !connected;
		}

		private void UpdateStateInfoText(IUnitySocketClient socketClient)
		{
			if (socketClient == null)
			{
				return;
			}

			var text = $"Server ip: {socketClient.Endpoint.Address}, Server port: {socketClient.Endpoint.Port}\n" +
			           $"Connecting: {socketClient.IsConnecting}, IsConnected: {socketClient.IsConnected}\n ";
			stateInfoText.text = text;
		}

		private void TriggerTcpConnect()
		{
			Connect(ExampleEchoClient.Type.Tcp);
		}

		private void TriggerDisconnect()
		{
			client.Disconnect();
		}

		private void Connect(ExampleEchoClient.Type type)
		{
			var serverIp = "127.0.0.1";
			var serverPort = 1111;
			client.ApplyInputAndConnect(serverIp, serverPort, type);
		}

		[ContextMenu("Send message")]
		private void OnSendEcho()
		{
			var message = Encoding.UTF8.GetBytes(messageInputField.text);
			client.SendEcho(message);
		}

		public void AddResponseText(string messages)
		{
			serverResponseText.text = messages + serverResponseText.text;
		}
	}
}
