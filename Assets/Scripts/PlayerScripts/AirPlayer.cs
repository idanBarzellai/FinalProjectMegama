using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlayer : BasicsController
{

    private float skillDuration = 3f;
    bool gettingLargerScale = false;
    bool gravityShouldBeStopped = false;
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
            Fly();

            AirBurstEffect();
            
        }
    }

    protected override void SkillTrigger()
    {

        AirSkillJumpStart();

        base.SkillTrigger();

        ImpactArea.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
        photonView.RPC("triggerEffect", RpcTarget.All);
        StartCoroutine(AirBurstHandler());
        
        photonView.RPC("SetAnim", RpcTarget.All, "Skill");


        StartCoroutine(ShouldStopGravity());
    }



    private void Fly()
    {
        if (GetInSkill() && !isGrounded && gravityShouldBeStopped && rb.velocity.y <= -0.05)
        {
            StartCoroutine(FlyCo());

        }

    }

    private IEnumerator ShouldStopGravity()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        gravityShouldBeStopped = true;
    }
    private IEnumerator FlyCo()
    {
        StopGravity();
        SetSpeed(flightSpeed);
        yield return new WaitForSecondsRealtime(skillDuration);
        
        resetVariables();
    }
    void StopGravity()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private void resetVariables()
    {
        SetInSkill(false);
        rb.useGravity = true;
        gravityShouldBeStopped = false;
        SetSpeed(moveSpeed);
        photonView.RPC("SetAnim", RpcTarget.All, "SkillEnd");
    }

    private void AirBurstEffect()
    {
        if (ImpactArea.transform.localScale.x <= maxRadius && gettingLargerScale)
        {
            ImpactArea.transform.localScale += new Vector3(0.5f, 0, 0.5f);
        }
    }
    private IEnumerator AirBurstHandler(float dmg = 1f)
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
    private void AirSkillJumpStart()
    {
        Debug.Log("flying");

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        setGrounded(false);

    }
    protected override bool IsApplingDownForce()
    {
        return !isGrounded && rb.useGravity;
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
