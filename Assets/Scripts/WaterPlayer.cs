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

    public GameObject impactArea;
    private GameObject impactAreaInstance;

    protected override void Start()
    {
        base.Start();
        UIController.instance.skillSliderFillColor.color = Color.blue;


    }
    protected override void Update()
    {
        base.Update();
        if (photonView.IsMine)
        {
            //if (Time.time - skillLastUseTime > skillCooldown && Input.GetKeyDown(KeyCode.Q))
            //{
            //    waterSkill();
            //    skillLastUseTime = Time.time;
            //}
            
            if (Input.GetKeyUp(KeyCode.Q))
                resetVariables();

            if (GetInSkill() && !skillCanceled && !playerInMidAir && rb.velocity.y <= 0.005)
                cancelGravity();

            if (impactAreaInstance != null)
            {
                if (GetIsStaticSkill())
                    transform.Translate(dir * GetActiveSpeed() * Time.deltaTime);
                impactAreaInstance.transform.Translate(dir * GetActiveSpeed() * Time.deltaTime);
            }
        }
    }


    protected override void SkillTrigger()
    {
        dir = transform.forward;
        dir = transform.InverseTransformDirection(dir);
        dir.y = 0;
        dir.Normalize();

        waterSkillHelper();
        Jump(skilljump);
    }


    private void waterSkillHelper()
    {
        skillCanceled = false;
        SetInSkill(true);
        SetIsStaticSkill(true);
        SetIsRotationStaticSkill(true);
        photonView.RPC("SetAnim", RpcTarget.All, "Skill");

        impactAreaInstance = PhotonNetwork.Instantiate(impactArea.name, transform.position - wavePos, transform.rotation);
        impactAreaInstance.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
        StartCoroutine(EndSkill());

    }

    private void resetVariables()
    {
        skillCanceled = true;
        SetInSkill(false);
        SetIsStaticSkill(false);
        SetIsRotationStaticSkill(false);

        photonView.RPC("SetAnim", RpcTarget.All, "SkillEnd");

        playerInMidAir = false;
        rb.useGravity = true;
    }

    private void cancelGravity()
    {
        playerInMidAir = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    private IEnumerator EndSkill()
    {
        yield return new WaitForSecondsRealtime(skillDuration);
        //Destroy(impactAreaInstance);

        if (!skillCanceled)
        {
            SetInSkill(false);
            resetVariables();
        }
        skillCanceled = false;

    }

    //private void OnTriggerStay(Collider other)
    //{
    //    // TODO need to be inside the water skill object prefab so it can deal damage properly and apply push force.
    //    if (other.CompareTag("Player") && !other.gameObject.GetPhotonView().IsMine)
    //    {
    //        Vector3 dir = impactArea.transform.forward;
    //        other.gameObject.GetPhotonView().RPC("PushedForce", RpcTarget.All, dir * pushForce);
    //        other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg , photonView.name);

    //    }
    //}
}
