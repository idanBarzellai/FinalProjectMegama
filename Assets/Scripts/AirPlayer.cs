using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPlayer : BasicsController
{

    private float skillDuration = 3f;
    float dmg = 3;
    bool playerInMidAir = false;
    bool gettingLargerScale = false;

    public GameObject ImpactArea;

    protected override void Update()
    {
        base.Update();
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q)) // Jump
                airSkill();

            if (!playerInMidAir && GetInSkill() && rb.velocity.y <= 0.005)
            {
                StartCoroutine(airSKillHelper());
            }

            if (ImpactArea.transform.localScale.x <= 6f && gettingLargerScale)
            {
                ImpactArea.transform.localScale += new Vector3(0.5f, 0, 0.5f);
            }
        }
    }

    private void airSkill()
    {
        SetInSkill(true);
        SetAnim("Skill");
        ImpactArea.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
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
        SetAnim("SkillEnd");
    }

    private IEnumerator createAOEDamage(float dmg = 1f)
    {
        // Change to close tiles
        ImpactArea.GetComponent<MeshRenderer>().enabled = true;
        ImpactArea.GetComponent<CapsuleCollider>().enabled = true;
        gettingLargerScale = true;
        yield return new WaitForSecondsRealtime(0.5f);
        gettingLargerScale = false;

        ImpactArea.GetComponent<MeshRenderer>().enabled = false;
        ImpactArea.GetComponent<CapsuleCollider>().enabled = false;
        ImpactArea.transform.localScale = new Vector3(1, 0.4f, 1);


    }

    
}
