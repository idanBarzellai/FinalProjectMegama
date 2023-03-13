using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class SkillHelper2 : MonoBehaviour
{
    private Rigidbody playerRb;

    private Vector3 origin;
    Vector3 dir;
    private float skillDuration = 3f;
    public CharcterController player;
    bool playerInMidAir = false;
    bool skillCanceled = false;
    bool waterSkillActive = false;


    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (waterSkillActive)
        {
            if (!skillCanceled && !playerInMidAir && player.GetInSkill() && playerRb.velocity.y <= 0.005)
                StartCoroutine(airSKillHelper());

            if (player.GetIsStaticSkill())
                player.transform.Translate(dir * player.GetActiveSpeed() * Time.deltaTime);
            transform.Translate(dir * player.GetActiveSpeed() * Time.deltaTime);
        }
    }



    public void waterSkill()
    {
        dir = player.transform.forward;
        dir = player.transform.InverseTransformDirection(dir);
        dir.y = 0;
        dir.Normalize();

        waterSkillHelper();
        StartCoroutine(createAOEDamage());
        player.Jump(2);
    }

    private void waterSkillHelper()
    {
        skillCanceled = false;
        player.SetInSkill(true);
        player.SetIsStaticSkill(true);
        player.SetIsRotationStaticSkill(true);
        waterSkillActive = true;
        player.SetAnim("AirSkill");  // change to water anim
    }

    public void resetVariables()
    {
        skillCanceled = true;

        player.SetIsStaticSkill(false);
        player.SetIsRotationStaticSkill(false);
        //player.freezeMovement(false);

        player.SetAnim("AirSkillEnd");

        playerInMidAir = false;
        playerRb.useGravity = true;
        Debug.Log("enables gravity, right? :" + playerRb.useGravity);
    }

    private IEnumerator airSKillHelper()
    {
        Debug.Log("hey");
        playerInMidAir = true;
        playerRb.useGravity = false;
        playerRb.velocity = Vector3.zero;
        yield return new WaitForSecondsRealtime(skillDuration);
        resetVariables();
    }

    public IEnumerator createAOEDamage(float dmg = 1f)
    {
        // Change to close tiles
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
        transform.position = player.transform.position - new Vector3(0 , -1.5f, 0);
        transform.rotation = player.transform.rotation;

        yield return new WaitForSecondsRealtime(3.5f);
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        player.SetInSkill(false);

        waterSkillActive = false;


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 dir = this.transform.forward;
            other.gameObject.GetComponent<Rigidbody>().AddForce(dir * 6, ForceMode.Impulse);
        }
    }
}
