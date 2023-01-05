using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaising : MonoBehaviour
{
    [SerializeField] private float rts = 200f;
    [SerializeField] private float speed;
    private GameObject player;
    private Rigidbody2D rigidbody;
    private bool startChasing = false;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(startChasing == true)
        {
            Vector2 pt = (Vector2)transform.position - (Vector2)player.transform.position;
            pt.Normalize();
            float x = Vector3.Cross(pt, transform.right).z;
            rigidbody.angularVelocity = rts * x;
            rigidbody.velocity = transform.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GameController") { startChasing = true; player = collision.gameObject; }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "GameController") { startChasing = true; player = collision.gameObject; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "GameController") startChasing = false;
    }
}
