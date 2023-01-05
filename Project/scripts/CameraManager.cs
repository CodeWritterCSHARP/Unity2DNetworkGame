using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CameraManager : MonoBehaviourPunCallbacks
{
    private float playerpos1;
    private float playerpos2;

    private float olddist;

    private Camera camera;
    [SerializeField] private GameObject fluidCamera;
    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        camera = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        float size = camera.orthographicSize;
        fluidCamera.GetComponent<Camera>().orthographicSize = size * 2;
        fluidCamera.transform.GetChild(0).gameObject.transform.localScale = new Vector3(size * 4, size * 4, 1);

        if (playerpos1 == 0 || playerpos2 == 0)
            transform.position = Vector3.Lerp(transform.position, new Vector3(GameObject.FindGameObjectWithTag("GameController").transform.position.x, transform.position.y, transform.position.z), .25f);

        if (!view.IsMine) return;
        view.RPC("CameraPosChange", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void CameraPosChange()
    {
        try {
            playerpos1 = (float)PhotonNetwork.CurrentRoom.CustomProperties["PlayerPos1"];
            playerpos2 = (float)PhotonNetwork.CurrentRoom.CustomProperties["PlayerPos2"];
        }
        catch { return; }

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            float middlepos = playerpos1 + (playerpos2 - playerpos1) / 2;
            transform.position = Vector3.Lerp(transform.position, new Vector3(middlepos, transform.position.y, transform.position.z), .25f);

            float distance = Vector2.Distance(new Vector2(playerpos1, transform.position.y), new Vector2(playerpos2, transform.position.y));
            if (distance > 16)
            {
                if (distance < olddist - 0.5) while (distance / 2 < camera.orthographicSize) camera.orthographicSize -= 0.1f;
                if (distance > olddist + 0.5) while (distance / 2 > camera.orthographicSize) camera.orthographicSize += 0.1f;
                olddist = distance;
            }
            else camera.orthographicSize = 8;
        }
        else
        {
            if (playerpos1 != 0)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(playerpos1, transform.position.y, transform.position.z), .25f);
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "PlayerPos2", 0 } });
            }
            if (playerpos2 != 0)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(playerpos2, transform.position.y, transform.position.z), .25f);
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "PlayerPos1", 0 } });
            }
           // if (playerpos1 == 0 || playerpos2 == 0) transform.position = new Vector2(GameObject.FindGameObjectWithTag("GameController").transform.position.x, transform.position.y);
        }
    }
}
