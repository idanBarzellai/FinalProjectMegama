using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHelper : MonoBehaviour
{
    private Rigidbody playerRb;
    
    private float skillDuration = 3f;
    public CharcterController player;
    float pushForce = 6f;
    bool playerInMidAir = false;
    bool gettingLargerScale = false;
    bool airSkillActive = false; // unnecessary when only one power , debug usage

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (airSkillActive)
        {
            if (!playerInMidAir && player.GetInSkill() && playerRb.velocity.y <= 0.005)
            {
                StartCoroutine(airSKillHelper());


            }

            if (this.gameObject.transform.localScale.x <= 6f && gettingLargerScale)
            {
                this.gameObject.transform.localScale += new Vector3(0.5f, 0, 0.5f);
            }
        }
    }

    public void airSkill()
    {
        airSkillActive = true;
        player.SetInSkill(true);
        player.SetAnim("AirSkill");
        StartCoroutine(createAOEDamage());
        player.Jump(2);
    }

    private IEnumerator airSKillHelper()
    {
        playerInMidAir = true;
        playerRb.useGravity = false;
        playerRb.velocity= Vector3.zero;
        yield return new WaitForSecondsRealtime(skillDuration);
        player.SetInSkill(false);
        playerInMidAir = false;
        playerRb.useGravity = true;
        player.SetAnim("AirSkillEnd");
        airSkillActive = false;

    }

    public IEnumerator createAOEDamage(float dmg = 1f)
    {
        // Change to close tiles
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        gettingLargerScale = true;
        yield return new WaitForSecondsRealtime(0.5f);
        gettingLargerScale = false;

        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        this.gameObject.transform.localScale = new Vector3(1, 0.4f, 1);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 dir = other.transform.position - this.transform.position;
            other.gameObject.GetComponent<Rigidbody>().AddForce(dir  *pushForce, ForceMode.Impulse);
        }
    }
}
