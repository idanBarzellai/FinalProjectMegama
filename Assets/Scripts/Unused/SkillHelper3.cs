using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SkillHelper3 : MonoBehaviour
{

    private float fireTrailDur = 5f;
    private float fireInstanceCreationRate = 0.3f;
    private int fireTrailCounter = 0;
    private int fireTrailMax = 10;
    private float runningSpeed = 15f;
    private float regSpeed = 8f;
    public CharcterController player;

    public GameObject fireTrail;


    private void Update()
    {
        if(fireTrailCounter== fireTrailMax) {
            CancelInvoke("fireSkillHelper");
            player.SetSpeed(regSpeed);
            player.SetInSkill(false);
            player.SetAnim("AirSkillEnd");
            fireTrailCounter= 0;

        }
    }


    public void fireSkill()
    {
        player.SetInSkill(true);
        player.SetSpeed(runningSpeed);
        player.SetAnim("AirSkill");
        InvokeRepeating("fireSkillHelper", 0.5f, fireInstanceCreationRate);
    }

    private void fireSkillHelper()
    {
        GameObject fireTrailInstance = Instantiate(fireTrail, player.transform.position, Quaternion.identity);
        Destroy(fireTrailInstance, fireTrailDur);
        fireTrailCounter++;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy")) {
            Debug.Log("Do dps");
        }
    }
}
