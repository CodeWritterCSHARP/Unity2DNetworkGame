using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Grab : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isGrabbing = false;
    private Transform parent;
    [SerializeField] private Transform grabpoint;
    Vector3 dist;

    Vector3 realposition;
    float changetimer = 1f;

    private PhotonView view;

    private void Awake() => view = GetComponent<PhotonView>();
    private void Start() => parent = GameObject.FindGameObjectWithTag("Hand").transform.parent.transform;

    private void Update()
    {
        if (parent.GetComponent<CapsuleCollider2D>().enabled == false)
        {
            if (view.IsMine)
            {
                if (isGrabbing == true)
                {
                    changetimer -= 0.25f;
                    if (changetimer <= 0) { changetimer = 0; UnGrabbing(); }
                    parent.position = grabpoint.position;
                }
            }
            else
            {
                parent.transform.position = Vector3.Lerp(parent.transform.position, realposition, .15f);
            }
        }
        else realposition = new Vector3();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Hand" && view.IsMine) Doing(collision);
    }

    void UnGrabbing()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (Transform child in parent) { child.position -= dist; }
            isGrabbing = false;
            changetimer = 1;
            //parent.GetComponent<CapsuleCollider2D>().enabled = true;
            view.RPC("ColliderChange", RpcTarget.AllBuffered, true);
        }
    }

    [PunRPC]
    void ColliderChange(bool status)
    {
        if (parent != null)
        {
            parent.GetComponent<CapsuleCollider2D>().enabled = status;
        }
    }
   
    void Doing(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.R) && isGrabbing == false)
        {
            parent = collision.transform.parent;
            parent.transform.position = grabpoint.transform.position;

            dist = parent.position - collision.transform.position;

            foreach (Transform child in parent) { child.position += dist; }

           // parent.GetComponent<CapsuleCollider2D>().enabled = false;
            view.RPC("ColliderChange", RpcTarget.AllBuffered, false);
            isGrabbing = true;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (parent != null) 
        {
            if (stream.IsWriting) stream.SendNext(parent.transform.position);
            else realposition = (Vector3)stream.ReceiveNext();
        }
    }
}
