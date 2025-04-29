using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coin : PowerupBaseController
{
    public SphereCollider col;
    Rigidbody rb;
    BasicsController player;
    bool goToPlayer = false;
    bool canGoToPlayer = false;
    float speed = 50;
    float fallMultiplier = 2;
    float downForce = 2;


    protected override void Start()
    {
        base.Start();
        transform.localScale = transform.localScale * Random.Range(1,5);
        rb = GetComponent<Rigidbody>();
        Invoke("coinTossFromPlayer", 0.05f);
        Invoke("GoToPlayer", 1.5f);
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.y <= 0)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * downForce * Time.deltaTime;
        //rb.AddForce(Vector3.down * fallMultiplyer, ForceMode.Force);

        else
            // rb.AddForce(Vector3.down * fallMultiplyer * downForce, ForceMode.Force);
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        if (goToPlayer)
        {
            rb.linearVelocity = (player.transform.position - transform.position).normalized * speed;
            //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .03f);
        }
    }
    public void SetPlayer(BasicsController _player)
    {
        player = _player;
    }
    public void GoToPlayer()
    {
        goToPlayer = true;
        col.enabled = false;

    }

    public void coinTossFromPlayer()
    {
        Vector3 force = new Vector3(UnityEngine.Random.Range(-1f, 1f), 2, UnityEngine.Random.Range(-1f, 1f));
       rb.AddForce(force *2, ForceMode.Impulse);
        canGoToPlayer = true;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && canGoToPlayer)
        {
            GoToPlayer();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (player)
        {

            if (other.gameObject == player.gameObject && canGoToPlayer)
            {
                Destroy(this.gameObject);
                Rigidbody rb = GetComponent<Rigidbody>();
                Destroy(rb);
            }
        }
        if (other.gameObject.CompareTag("Ground") && canGoToPlayer)
        {
            GoToPlayer();
        }
    }

}
