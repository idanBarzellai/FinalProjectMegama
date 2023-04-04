using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthPlayer : BasicsController
{
    public LineRenderer lineRenderer;

    private int linePoints = 25;
    private float timeBetweenpoints = 0.1f;
    public LayerMask floorCollisionMask;

    public GameObject ShowLandImpact;
    public GameObject impactArea;
    public ParticleSystem impactAreaEffect;

    Vector3 dir;

    bool playerLeapedFromGround = false;
    bool skillTriggered = false;
    float basicFallForce;
    float addedFallForce;

    float forwardForce = 40;
    float upwordForce = 120;


    protected override void Start()
    {
        base.Start();
        basicFallForce = fallMultiplyer;
        addedFallForce = fallMultiplyer * 4;//(2 *jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        //upwordForce = addedFallForce * timeToJumpApex;
        if(photonView.IsMine)
            UIController.instance.skillSliderFillColor.color = Color.magenta;


    }
    protected override void Update()
    {
        base.Update();
        if (photonView.IsMine)
        {
            //rb.AddForce(Vector3.up * -fallMultiplyer, ForceMode.Acceleration);

            if (!skillTriggered && GetInSkill() && Input.GetKeyUp(KeyCode.Q) && isGrounded)
            {
                skillTriggered = true;

                earthSkill();
            }
        }
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (photonView.IsMine)
        {
            if (lineRenderer.enabled)
            {
                DrawProjection();
            }
            if (playerLeapedFromGround && !isGrounded)
            {
                lineRenderer.enabled = false;
                ShowLandImpact.SetActive(false);
            }
            if (GetInSkill() && !lineRenderer.enabled && playerLeapedFromGround && isGrounded)
            {
                impactArea.GetComponent<SkillInstanceController>().SetName(photonView.Owner.NickName);
                photonView.RPC("triggerEffect", RpcTarget.All);
                resetVariables();

                StartCoroutine(createAOEDamage());
                playerLeapedFromGround = false;

            }
        }

    }
    protected override void SkillTrigger()
    {
        base.SkillTrigger();
        lineRenderer.enabled = true;
        SetIsStaticSkill(true);
        SetFallMultiplyer(addedFallForce);
    }


    private void DrawProjection()
    {
        // Should everyone see where the player is gonna land?
        dir = transform.forward;
        dir = transform.InverseTransformDirection(dir);
        dir.Normalize();


        lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenpoints) + 1;
        Vector3 startPosition = transform.position;
        Vector3 startVelocity = (transform.forward * forwardForce + Vector3.up * upwordForce );
        //startVelocity.y -= (Mathf.Abs(fallMultiplyer) + Mathf.Abs(Physics.gravity.y));//+ new Vector3(0, jumpVelocity, 0)) / (fallMultiplyer);
        int i = 0;
        lineRenderer.SetPosition(i, startPosition);

        for (float time = 0; time < linePoints; time += timeBetweenpoints) // 250 points
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + 0.5f * (-(Mathf.Abs(fallMultiplyer) + Mathf.Abs(Physics.gravity.y)) * time * time);

            lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = lineRenderer.GetPosition(i - 1);
            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                5,
                floorCollisionMask))
            {
                lineRenderer.SetPosition(i, hit.point);
                lineRenderer.positionCount = i + 1;

                ShowLandImpact.transform.position = lastPosition;
                ShowLandImpact.SetActive(true);
                return;
            }
        }
    }

    public void earthSkill()
    {
        playerLeapedFromGround = true;

        //rb.AddForce(transform.forward * forwardForce + new Vector3(0, upwordForce * 1.5f, 0), ForceMode.Impulse);
        rb.AddForce(transform.forward * forwardForce +  Vector3.up * upwordForce, ForceMode.VelocityChange);
        photonView.RPC("SetAnim", RpcTarget.All, "Skill");

    }

    public IEnumerator createAOEDamage()
    {

        //impactArea.GetComponent<MeshRenderer>().enabled = true;
        impactArea.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSecondsRealtime(0.5f);

        //impactArea.GetComponent<MeshRenderer>().enabled = false;
        impactArea.GetComponent<SphereCollider>().enabled = false;
    }

    private void resetVariables()
    {
        SetFallMultiplyer(basicFallForce);
        photonView.RPC("SetAnim", RpcTarget.All, "SkillEnd");
        SetIsStaticSkill(false);
        SetInSkill(false);
        skillTriggered = false;


    }



    [PunRPC]
    public void triggerEffect()
    {
        impactAreaEffect.Play();

    }
}
