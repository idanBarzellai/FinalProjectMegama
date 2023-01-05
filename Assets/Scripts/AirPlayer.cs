using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlayer : BasicsController
{

    private float skillDuration = 3f;
    bool playerInMidAir = false;
    bool gettingLargerScale = false;
    float maxRadius = 6f;

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

            if (!playerInMidAir && GetInSkill() && rb.velocity.y <= 0.005)
            {
                StartCoroutine(airSKillHelper());
            }

            if (ImpactArea.transform.localScale.x <= maxRadius && gettingLargerScale)
            {
                ImpactArea.transform.localScale += new Vector3(0.5f, 0, 0.5f);
            }
        }
    }

    protected override void SkillTrigger()
    {
        base.SkillTrigger();
        photonView.RPC("SetAnim", RpcTarget.All, "Skill");
        ImpactArea.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
        photonView.RPC("triggerEffect", RpcTarget.All);

        StartCoroutine(createAOEDamage());
        Jump();
    }

    private IEnumerator airSKillHelper()
    {
        playerInMidAir = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        yield return new WaitForSecondsRealtime(skillDuration);
        
        resetVariables();
    }

    private void resetVariables()
    {
        SetInSkill(false);
        playerInMidAir = false;
        rb.useGravity = true;
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

    public float GetAirWaveRadius()
    {
        return maxRadius;
    }

    [PunRPC]
    public void triggerEffect()
    {
        impactAreaEffect.Play();

    }
}
