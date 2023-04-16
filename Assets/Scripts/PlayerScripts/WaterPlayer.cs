using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class WaterPlayer : BasicsController
{
    
    Vector3 dir;
    Vector3 wavePos = new Vector3(0,-2, 0);
    private float skillDuration = 3f;
    float skilljump = 0.7f;
    bool playerInMidAir = false;
    bool skillCanceled = false;
    float stopGravityThershold = -0.005f;
    float JumpDuration = 0.6f;
    bool gravityShouldBeStopped = false;


    public GameObject impactArea;
    private GameObject impactAreaInstance;

    protected override void Start()
    {
        base.Start();
        SetSkillBarColor(Color.blue);
    }
    protected override void Update()
    {
        base.Update();
        if (amIPlayingAndNotDead())
        {
            WaterSKill();
        }
    }

    private void WaterSKill()
    {
        if (Input.GetKeyUp(KeyCode.Q))
            resetVariables();


        stopGravityForSkill();
        MoveWithWave();
    }

    protected override void SkillTrigger()
    {
        dir = transform.forward;
        dir = transform.InverseTransformDirection(dir);
        dir.y = 0;
        dir.Normalize();

        waterSkillHelper();
        shouldJump = true;
        StartCoroutine(ShouldStopGravity());
    }


    private void waterSkillHelper()
    {
        skillCanceled = false;
        SetInSkill(true);
        SetIsStaticSkill(true);
        SetIsRotationStaticSkill(true);
        photonView.RPC("SetAnim", RpcTarget.All, "Skill");

        CreateWave();

        StartCoroutine(EndSkillCo());
    }

    private void CreateWave()
    {
        impactAreaInstance = PhotonNetwork.Instantiate(impactArea.name, transform.position - wavePos, transform.rotation);
        impactAreaInstance.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
        impactAreaInstance.GetComponent<SkillInstanceController>().SetPlayer(this);
        impactAreaInstance.GetComponent<WaveInstanceController>().Move(dir * GetActiveSpeed());

        SkillManager.instance.DestoryOverNetwork(5f, impactAreaInstance);
    }

    private void resetVariables()
    {
        skillCanceled = true;
        SetInSkill(false);
        SetIsStaticSkill(false);
        SetIsRotationStaticSkill(false);

        photonView.RPC("SetAnim", RpcTarget.All, "SkillEnd");

        gravityShouldBeStopped = false;
        rb.useGravity = true;
    }

    private void stopGravityForSkill()
    {
        if (GetInSkill() && !skillCanceled && gravityShouldBeStopped && rb.velocity.y <= stopGravityThershold)
        {
            
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
    }
    private IEnumerator ShouldStopGravity()
    {
        yield return new WaitForSecondsRealtime(JumpDuration);
        gravityShouldBeStopped = true;
    }

   
    private void MoveWithWave()
    {
        if (impactAreaInstance != null)
        {
            if (GetIsStaticSkill())
                transform.Translate(dir * GetActiveSpeed() * impactAreaInstance.GetComponent<WaveInstanceController>().waveSpeed * Time.deltaTime);

        }
    }
    protected override bool IsApplingDownForce()
    {
        return !isGrounded && rb.useGravity;
    }

    private IEnumerator EndSkillCo()
    {
        yield return new WaitForSecondsRealtime(skillDuration);

        if (!skillCanceled)
        {
            SetInSkill(false);
            resetVariables();
        }
        skillCanceled = false;

    }
}
