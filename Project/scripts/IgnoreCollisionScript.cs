using UnityEngine;

public class IgnoreCollisionScript : MonoBehaviour
{
    [SerializeField] private string layer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer(layer))
            Physics2D.IgnoreCollision(collision.collider.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer(layer))
            Physics2D.IgnoreCollision(collision.collider.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
}
