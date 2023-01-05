using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviourPun
{
    [SerializeField] private GameObject Prefab;

    private Rigidbody2D projectile;

    private bool canShoot = false;
    private bool coroutuneCheck = true;

    private int playersInRange;

    private GameObject player;

    private PhotonView view;

    private IEnumerator ShootWaiting(float waitTime)
    {
        coroutuneCheck = false;
        yield return new WaitForSeconds(waitTime);
        coroutuneCheck = true;

        view.RPC("RotationChange", RpcTarget.AllBuffered);
        projectile = PhotonNetwork.Instantiate(Prefab.name, gameObject.transform.position, gameObject.transform.rotation).GetComponent<Rigidbody2D>();
        Vector3 Velocity = CalculatorOfVelocity(player.transform.position, projectile.position, 2f);
        projectile.velocity = Velocity;
    }

    private void Awake() => view = GetComponent<PhotonView>();

    [PunRPC]
    void RotationChange()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, -player.transform.rotation.y, transform.rotation.z);
    }

    private void Update()
    {
        if (canShoot == true && coroutuneCheck == true && view.IsMine) StartCoroutine(ShootWaiting(2));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GameController") { canShoot = true; player = collision.gameObject; }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "GameController") { canShoot = true; player = collision.gameObject; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "GameController") canShoot = false;
    }

    Vector3 CalculatorOfVelocity(Vector3 target, Vector3 origin, float time)
    {
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }
}
