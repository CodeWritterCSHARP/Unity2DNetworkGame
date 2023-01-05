using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;

public class SpawnPlayers : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("StartSettings")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float YPos = -0.62f;

    [SerializeField] private GameObject player;
    Vector2 spawnpoint = Vector2.zero;

    [Header("PlayerList")]
    [SerializeField] private GameObject playerListPrefab;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private Text roomname;

    [Header("Timer")]
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject StartBt;
    [SerializeField] private float currCountdownValue = 4f;
    private float Timer;
    private bool canchange = false;


    private PhotonView view;

    private void Awake() => view = GetComponent<PhotonView>();

    private void Start() {
        Timer = currCountdownValue;
        if (!PhotonNetwork.IsMasterClient) StartBt.SetActive(false);
        SpawnPlayer();
        ListUpdate();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if((bool)PhotonNetwork.CurrentRoom.CustomProperties["CanStart"] == true) startPanel.SetActive(false);
            return;
        }

        if (canchange == true)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "CanStart", true } });
                if (Timer < -0.5f) view.RPC("TurnOff", RpcTarget.AllBuffered);
            }
            else timer.GetComponent<Text>().text = Timer.ToString("F1");
        } else if(PhotonNetwork.IsMasterClient && StartBt.activeSelf == false) StartBt.SetActive(true);
    }

    void SpawnPlayer()
    {
        Player[] players = PhotonNetwork.PlayerList;
        spawnpoint = new Vector2(Random.Range(minX, maxX), YPos);

        if (players.Length <= 1) 
        {
            PhotonNetwork.Instantiate(player.name, spawnpoint, Quaternion.identity);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "Position", spawnpoint.x} });
        }
        else
        {
            Vector2 vector = new Vector2((float)PhotonNetwork.CurrentRoom.CustomProperties["Position"], YPos);

            if (Vector2.Distance(vector, spawnpoint) > 3.25f) PhotonNetwork.Instantiate(player.name, spawnpoint, Quaternion.identity);
            else SpawnPlayer();
        }
    }

    private void ListUpdate()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
           Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerList>().SetUp(players[i]);
        roomname.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void StartBTN()
    {
        if (PhotonNetwork.PlayerList.Length >= 2) view.RPC("Starting", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void Starting()
    {
        timer.SetActive(true);
        startPanel.SetActive(false);
        canchange = true;
    }

    [PunRPC]
    private void TurnOff()
    {
        timer.SetActive(false);
        canchange = false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerList>().SetUp(newPlayer);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
       // spawnpoint = 
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient) stream.SendNext(Timer);
        else if (stream.IsReading)
        {
            Timer = (float)stream.ReceiveNext();
            if (Timer >= 0 && canchange == true) timer.GetComponent<Text>().text = Timer.ToString("F1");
        }
    }
}
