using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using ExitGames.Client.Photon;
using System;

public class ChatManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    [SerializeField] string userID;
    [SerializeField] Text chatText;
    [SerializeField] InputField textMessage;

    ChatClient chatClient;

    PhotonView view;

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"{level},{message}");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state);
    }

    public void OnConnected()
    {
        chatText.text += Environment.NewLine + "You have been joing";
        chatClient.Subscribe(PhotonNetwork.CurrentRoom.Name);
    }

    public void OnDisconnected()
    {
        chatClient.Unsubscribe(new string[] { PhotonNetwork.CurrentRoom.Name });
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            chatText.text += Environment.NewLine + $"[{channelName}] {senders[i]}: {messages[i]}";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            chatText.text += Environment.NewLine + $"U have joined to chanel {channels[i]}";
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            chatText.text += Environment.NewLine + $"You have unjoined from chanel {channels[i]}";
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        chatText.text += Environment.NewLine + $"User {user} joined to chanel {channel}";
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        chatText.text += Environment.NewLine + $"User {user} unjoined from chanel {channel}";
    }

    private void Awake() => view = GetComponent<PhotonView>();

    private void Start()
    {
        chatClient = new ChatClient(this);
        userID = PhotonNetwork.NickName;
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new Photon.Chat.AuthenticationValues(userID));
    }

    private void Update()
    {
        chatClient.Service();
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(textMessage.text))
        {
            if (textMessage.text.Length <= 50) chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, textMessage.text);
            else textMessage.text = "Max symols count is 50 for a single message";
        }
        else textMessage.text = "A field cant be empty, write smth";
    }

    [PunRPC]
    private void ChatClearing()
    {
        chatText.text = null;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        view.RPC("ChatClearing", RpcTarget.AllBuffered);
    }

    public override void OnLeftRoom()
    {
        view.RPC("ChatClearing", RpcTarget.AllBuffered);
    }
}
