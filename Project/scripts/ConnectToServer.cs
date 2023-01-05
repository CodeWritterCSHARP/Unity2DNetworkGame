using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject disableObject;
    [SerializeField] private GameObject EnableObject;
    [SerializeField] private InputField PlayerName;

    public void TransitionBtwScenes()
    {
        if (!string.IsNullOrEmpty(PlayerName.text))
        {
            disableObject.SetActive(false);
            EnableObject.SetActive(true);
            PhotonNetwork.NickName = PlayerName.text;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(PlayerName.text)) TransitionBtwScenes();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
