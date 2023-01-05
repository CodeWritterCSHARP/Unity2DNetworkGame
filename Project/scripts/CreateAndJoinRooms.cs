using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField Join;
    [SerializeField] private InputField Create;

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        Hashtable RoomCustomProperties = new Hashtable();

        RoomCustomProperties.Add("PlayerPos1", 0f);
        RoomCustomProperties.Add("PlayerPos2", 0f);

        RoomCustomProperties.Add("Position", 0);

        RoomCustomProperties.Add("CanStart", false);
        roomOptions.CustomRoomProperties = RoomCustomProperties;

        PhotonNetwork.CreateRoom(Create.text, roomOptions);
    }
    public void JoinRoom() => PhotonNetwork.JoinRoom(Join.text);

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("SampleScene");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(Join.text) && string.IsNullOrEmpty(Create.text)) JoinRoom();
            if (!string.IsNullOrEmpty(Create.text) && string.IsNullOrEmpty(Join.text)) CreateRoom();
        }
    }
}
