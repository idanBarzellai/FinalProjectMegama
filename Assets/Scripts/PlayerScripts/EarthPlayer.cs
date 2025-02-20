using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthPlayer : BasicsController
{
    public LineRenderer lineRenderer;

    private int linePoints = 25;
    private float timeBetweenpoints = 0.1f;
    private LayerMask floorCollisionMask;

    public GameObject ShowLandImpact;
    public GameObject impactArea;
    public ParticleSystem impactAreaEffect;

    Vector3 dir;

    bool playerLeapedFromGround = false;
    bool skillTriggered = false;
    float basicFallForce;
    float addedFallForce;

    float forwardForce = 50;
    float upwordForce = 130;
    float distanceCalc = 20;


    protected override void Start()
    {
        base.Start();

        // This would cast rays only against colliders in layer 8.
        int layerMask = 1 << 3;
        floorCollisionMask = layerMask;
        
        basicFallForce = fallMultiplyer;
        addedFallForce = fallMultiplyer *3;//(2 *jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        //upwordForce = addedFallForce * timeToJumpApex;

        SetSkillBarColor(new Color(167, 74, 8, 255)); //Brown color
    }
    protected override void Update()
    {
        base.Update();
        if (amIPlayingAndNotDead())
        {
            //rb.AddForce(Vector3.up * -fallMultiplyer, ForceMode.Acceleration);
            earthSkill();
            
        }
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (amIPlayingAndNotDead())
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
        //dir = transform.InverseTransformDirection(dir);
        //dir.Normalize();

        GameObject myTile = FindTileUnderMe();
        GameObject endTile;

        if (myTile)
        {
            endTile = FindTileToLand(myTile.transform.position, dir);
            if (endTile)
            {
                ShowLandImpact.transform.position = endTile.transform.position + new Vector3(0, 10,0);
                ShowLandImpact.SetActive(true);
            }
            else { return; }

            DrawLine(transform.position, endTile.transform.position);

        }


    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenpoints) + 1;
        Vector3 startVelocity = (transform.forward * (forwardForce/2) + Vector3.up * (upwordForce/4));
        Debug.Log(startVelocity);
        //startVelocity.y -= (Mathf.Abs(fallMultiplyer) + Mathf.Abs(Physics.gravity.y));//+ new Vector3(0, jumpVelocity, 0)) / (fallMultiplyer);
        int i = 0;
        lineRenderer.SetPosition(i, start);

        for (float time = 0; time < linePoints; time += timeBetweenpoints) // 250 points
        {
            i++;

            Vector3 point = start + time * startVelocity;
            point.y = start.y + startVelocity.y * time + 0.5f * (-(Mathf.Abs(fallMultiplyer) + Mathf.Abs(Physics.gravity.y)) * time * time);

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
            }
        }
    }

    private GameObject FindTileUnderMe()
    {
        
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100, floorCollisionMask))
            return hit.collider.gameObject;

        return null;
        
    }

    private GameObject FindTileToLand(Vector3 startTilePos, Vector3 dir)
    {
        // Q = P + (d / ||v||) * (v / ||v||)
        // Q - end point
        // P - start tile point
        // d - distance
        // v - dir
        float d = (forwardForce) * Mathf.Sqrt(distanceCalc / addedFallForce);
        float magV = dir.magnitude;
        
        Vector3 endTilePos = startTilePos + (d / magV) * (dir / magV);

        RaycastHit hit;

        if (Physics.Raycast(endTilePos, Vector3.down, out hit, 100, floorCollisionMask))
        {

            return hit.transform.gameObject;
        }
        else if(Physics.Raycast(endTilePos, Vector3.up, out hit, 100, floorCollisionMask))
        {
            return hit.transform.gameObject;
        }
        
        return null;
    }

    public void earthSkill()
    {
        if (!skillTriggered && GetInSkill() && Input.GetKeyUp(KeyCode.Q))// && isGrounded) // TODO 
        {
            skillTriggered = true;
            playerLeapedFromGround = true;

            //rb.AddForce(transform.forward * forwardForce + new Vector3(0, upwordForce * 1.5f, 0), ForceMode.Impulse);
            rb.AddForce(transform.forward * forwardForce + Vector3.up * upwordForce, ForceMode.VelocityChange); // TODO forcemode.impulse
            photonView.RPC("SetAnim", RpcTarget.All, "Skill");
        }
        

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
