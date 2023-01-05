using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterForce : MonoBehaviour
{
    public LayerMask Ground;

    [SerializeField] private float force = 10;
    [SerializeField] private float timer = 2f;
    [SerializeField] private bool CanSwim;
    [SerializeField] private bool IsGround;

    private bool InWater = false;
    private List<GameObject> collist = new List<GameObject>();

    private CapsuleCollider2D capsuleCollider;
    private PhotonView view;

    private void Start()
    {
        if (GetComponent<PhotonView>() != null) view = GetComponent<PhotonView>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Update()
    {
        if (InWater == true) timer -= Time.deltaTime;
        if (timer <= 0) { InWater = false; timer = Random.Range(2.25f, 3); }

        if (CanSwim) IsGround = Physics2D.OverlapCircle(gameObject.transform.position, 2, Ground);
    }

    private void AddForcePower()
    {
        gameObject.transform.parent.GetComponent<Rigidbody2D>().AddForce(transform.up * force);
        foreach (Transform child in gameObject.transform.parent)
            child.GetComponent<Rigidbody2D>().AddForce(transform.up * 5);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            if (!collist.Contains(collision.gameObject)) collist.Add(collision.gameObject);

            if (CanSwim) {
                if (collist.Count > 2)
                {
                    InWater = true;

                    if(timer <= 3 && timer > 2f) AddForcePower();
                }
            }
            else
            {
                Debug.Log(collist.Count);
                if (capsuleCollider.enabled == true && collist.Count > 5)
                {
                    view.RPC("GrabDisable", RpcTarget.AllBuffered, false);
                    if(IsGround == false) Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
                    else Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), false);
                    GetComponent<Rigidbody2D>().gravityScale = 0.4f;
                }
            }
        }
    }

    [PunRPC]
    void GrabDisable(bool variable)
    {
        if(variable == false)
            foreach (Transform child in gameObject.transform) child.GetComponent<CapsuleCollider2D>().isTrigger = false;
        else foreach (Transform child in gameObject.transform) child.GetComponent<CapsuleCollider2D>().isTrigger = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            if (collist.Contains(collision.gameObject)) collist.Remove(collision.gameObject);

            if(CanSwim == false) 
            {
                if (capsuleCollider.enabled == true && collist.Count <= 5)
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), false);
                    GetComponent<Rigidbody2D>().gravityScale = 1f;
                    view.RPC("GrabDisable", RpcTarget.AllBuffered, true);
                }
            }
        }
    }
}
