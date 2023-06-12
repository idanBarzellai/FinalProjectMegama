using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlayer : BasicsController
{
    private float fireInstanceCreationRate = 0.3f;
    private int fireTrailCounter = 0;
    private int fireTrailMax = 10;
    private float fireTrailCreation = 0.2f;
    private float runningSpeed = 18f;
    private float regSpeed = 8f;

    public GameObject fireTrail;
    public List<GameObject> firetrailInstances = new List<GameObject>();
    protected override void Start()
    {
        base.Start();
        SetSkillBarColor(Color.red);
    }

    protected override void Update()
    {
        base.Update();
        if (amIPlayingAndNotDead())
            if (fireTrailCounter == fireTrailMax)
                resetVariables();
    }
    protected override void SkillTrigger()
    {
        base.SkillTrigger();
        SetSpeed(runningSpeed);
        photonView.RPC("SetAnimBool", RpcTarget.All, "InSkill", true);
        InvokeRepeating("fireSkillHelper", fireTrailCreation, fireInstanceCreationRate);
    }

    protected override bool IsAbleToJump()
    {
        return Input.GetKeyDown(KeyCode.Space) && isGrounded;
    }
    private void resetVariables()
    {
        CancelInvoke("fireSkillHelper");
        SetSpeed(regSpeed);
        SetInSkill(false);
        photonView.RPC("SetAnimBool", RpcTarget.All, "InSkill", false);
        fireTrailCounter = 0;
    }

   

    private void fireSkillHelper()
    {
        GameObject fireTrailInstance = PhotonNetwork.Instantiate(fireTrail.name, transform.position, transform.rotation);
        fireTrailInstance.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
        StartCoroutine(SkillManager.instance.DestroyOvertime(3f, fireTrailInstance));
        fireTrailCounter++;
    }
}
