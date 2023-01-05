using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInNetwork : MonoBehaviourPun
{
    [SerializeField] private int time;
    void Start() => GetComponent<PhotonView>().RPC("Destroing", RpcTarget.AllBuffered);

    [PunRPC]
    private void Destroing()
    {
        Destroy(gameObject, time);
    }
}
