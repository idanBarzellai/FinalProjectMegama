using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlayer : BasicsController
{

    private float skillDuration = 3f;
    bool gettingLargerScale = false;
    bool isStopGravity = false;
    float maxRadius = 6f;
    float flightSpeed = 25f;

    public GameObject ImpactArea;
    public ParticleSystem impactAreaEffect;

    protected override void Start()
    {
        base.Start();
        if (photonView.IsMine)
            UIController.instance.skillSliderFillColor.color = Color.white;


    }
    protected override void Update()
    {
        base.Update();

        if (photonView.IsMine)
        {

            if (isStopGravity && GetInSkill() && rb.velocity.y <= 0.005)
            {
                StartCoroutine(Fly());
            }

            if (ImpactArea.transform.localScale.x <= maxRadius && gettingLargerScale)
            {
                ImpactArea.transform.localScale += new Vector3(0.5f, 0, 0.5f);
            }
        }
    }

    protected override void SkillTrigger()
    {

        Jump();

        base.SkillTrigger();
        photonView.RPC("SetAnim", RpcTarget.All, "Skill");

        ImpactArea.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
        photonView.RPC("triggerEffect", RpcTarget.All);

        StartCoroutine(createAOEDamage());
        StartCoroutine(ShouldStopGravity());
    }
    private IEnumerator ShouldStopGravity()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        isStopGravity = true;
    }

    void StopGravity()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private IEnumerator Fly()
    {
        StopGravity();
        SetSpeed(flightSpeed);
        yield return new WaitForSecondsRealtime(skillDuration);
        
        resetVariables();
    }

    private void resetVariables()
    {
        SetInSkill(false);
        isStopGravity = false;
        rb.useGravity = true;
        SetSpeed(moveSpeed);
        photonView.RPC("SetAnim", RpcTarget.All, "SkillEnd");
    }

    private IEnumerator createAOEDamage(float dmg = 1f)
    {
        // Change to close tiles
        //ImpactArea.GetComponent<MeshRenderer>().enabled = true;

        ImpactArea.GetComponent<CapsuleCollider>().enabled = true;
        gettingLargerScale = true;
        yield return new WaitForSecondsRealtime(1f);
        gettingLargerScale = false;

        //ImpactArea.GetComponent<MeshRenderer>().enabled = false;
        ImpactArea.GetComponent<CapsuleCollider>().enabled = false;
        ImpactArea.transform.localScale = new Vector3(1f, 0.2f, 1f);


    }

    protected override bool IsApplingDownForce()
    {
        return !isGrounded && !GetInSkill();
    }

    protected override bool IsAbleToJump()
    {
        return (Input.GetKeyDown(KeyCode.Space)  || GetInSkill()) && isGrounded;
    }

    [PunRPC]
    public void triggerEffect()
    {
        impactAreaEffect.Play();

    }

    public float GetAirWaveRadius()
    {
        return maxRadius;
    }

}
