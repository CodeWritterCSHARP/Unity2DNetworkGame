using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviourPunCallbacks
{
    [SerializeField] private float bouncepower;
    [SerializeField] private float bouncepowerAddingValue;
    private float bouncepowerMaxValue;
    private float startbouncepower;
    private float timer = 5f;
    private bool startTimer = false;

    private void Start()
    {
        startbouncepower = bouncepower;
        bouncepowerMaxValue = bouncepower + bouncepowerAddingValue * 3;
    }

    private void FixedUpdate()
    {
        if (startTimer == true) timer -= Time.fixedDeltaTime; else timer = 5f;
        if (timer <= 0) { bouncepower = startbouncepower; startTimer = false;}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Trampoliini")
        {
            timer = 5f; startTimer = true;
            if (bouncepower < bouncepowerMaxValue) bouncepower += bouncepowerAddingValue;
            else bouncepower = startbouncepower;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * bouncepower, ForceMode2D.Impulse);
        }
    }
}
