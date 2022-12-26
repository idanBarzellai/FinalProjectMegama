using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlayer : BasicsController
{
    private float fireTrailDur = 5f;
    private float fireInstanceCreationRate = 0.3f;
    private int fireTrailCounter = 0;
    private int fireTrailMax = 10;
    private float runningSpeed = 18f;
    private float regSpeed = 8f;

    public GameObject fireTrail;
    public List<GameObject> firetrailInstances = new List<GameObject>();


    protected override void Update()
    {
        base.Update();
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                fireSkill();

            if (fireTrailCounter == fireTrailMax)
            {
                resetVariables();

            }
        }
    }

    private void resetVariables()
    {
        CancelInvoke("fireSkillHelper");
        SetSpeed(regSpeed);
        SetInSkill(false);
        SetAnim("SkillEnd");
        fireTrailCounter = 0;
    }

    private void fireSkill()
    {
        SetInSkill(true);
        SetSpeed(runningSpeed);
        SetAnim("Skill");
        InvokeRepeating("fireSkillHelper", 0.5f, fireInstanceCreationRate);
    }

    private void fireSkillHelper()
    {
        GameObject fireTrailInstance = PhotonNetwork.Instantiate(fireTrail.name, transform.position, transform.rotation);
        fireTrailInstance.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
        //Destroy(fireTrailInstance, fireTrailDur);
        fireTrailCounter++;
    }
}
