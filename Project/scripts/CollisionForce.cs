using Photon.Pun;
using UnityEngine;

public class CollisionForce : MonoBehaviour
{
    [SerializeField] private float ForcePower;
    [SerializeField] private bool isPunch;
    [SerializeField] private LayerMask Ground;

    private Rigidbody2D rigidbody;
    private PhotonView view;

    private void Start() => rigidbody = this.gameObject.GetComponent<Rigidbody2D>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        view = GetComponent<PhotonView>();
        if (collision.gameObject.tag == "GameController")
        {
            rigidbody.constraints = RigidbodyConstraints2D.None;
            Vector2 direction = collision.contacts[0].point - new Vector2(transform.position.x, transform.position.y);
            direction = -direction.normalized;
            ForcePower = Mathf.Abs(collision.gameObject.GetComponent<Rigidbody2D>().velocity.x);
            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(direction * ForcePower * 500);

            if(isPunch == false) Invoke("StopMovementInvoking", 6f);
        }
    }
    void StopMovementInvoking()
    {
        bool GroundChecker = Physics2D.OverlapCircle(gameObject.transform.position, 1f, Ground);

        if (Mathf.Abs(rigidbody.angularVelocity) <= 40 && GroundChecker == true)
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            isPunch = false;
        }
        else
        {
            isPunch = true;
            Invoke("StopMovementInvoking", 6f);
        }
    }
}
