using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHelper4 : MonoBehaviour
{
    public LineRenderer lineRenderer;

    [Range(10, 100)]
    private int linePoints = 25;
    [Range(0.01f, 0.25f)]
    private float timeBetweenpoints = 0.1f;

    private Rigidbody playerRb;

    private float skillDuration = 3f;
    public LayerMask floorCollisionMask;

    public CharcterController player;
    public GameObject hitArea;
    Vector3 dir;


    bool playerLeapedFromGround = false;
    bool earthSkillActive = false;
    float basicFallForce = 1.8f;
    float addedFallForce = 3f;


    float forwardForce = 10;
    float upwordFOrce = 20;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();

    }
    private void LateUpdate()
    {
        if (lineRenderer.enabled)
        {
            DrawProjection();
        }
        if(!playerLeapedFromGround && player.GetInSkill()  && player.isGrounded && earthSkillActive)
        {
            StartCoroutine(createAOEDamage());
        }
        if(playerLeapedFromGround && !player.isGrounded)
        {
            lineRenderer.enabled = false;
            hitArea.SetActive(false);

            playerLeapedFromGround = false;
        }
    }

    public void skillStarted()
    {
        skillIsActivated();
        lineRenderer.enabled = true;
    }

    private void DrawProjection()
    {

        dir = player.transform.forward;
        dir = player.transform.InverseTransformDirection(dir);
        dir.Normalize();

        
        lineRenderer.positionCount = Mathf.CeilToInt(linePoints / timeBetweenpoints) + 1;
        Vector3 startPosition = player.transform.position;
        Vector3 startVelocity = ( player.transform.forward * forwardForce + new Vector3(0, upwordFOrce, 0)) / (playerRb.mass * addedFallForce / 2);
        int i = 0;
        lineRenderer.SetPosition(i, startPosition);

        for (float time = 0; time < linePoints; time += timeBetweenpoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = lineRenderer.GetPosition(i - 1);
            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude,
                floorCollisionMask))
            {
                lineRenderer.SetPosition(i, hit.point);
                lineRenderer.positionCount = i + 1;

                hitArea.transform.position = lastPosition;
                hitArea.SetActive(true);
                return;
            }
        }

       

    }

    private void skillIsActivated()
    {
        player.SetInSkill(true);
        player.SetIsStaticSkill(true);
        player.SetFallMultiplyer(addedFallForce);
        player.SetAnim("AirSkill"); // Change to water
    }


    public void earthSkill()
    {
        earthSkillActive = true;

        playerRb.AddForce(player.transform.forward * forwardForce + new Vector3(0, upwordFOrce*1.5f , 0), ForceMode.Impulse);
        playerLeapedFromGround = true;
    }

    public IEnumerator createAOEDamage(float dmg = 1f)
    {
        // Change to close tiles

        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
        yield return new WaitForSecondsRealtime(0.5f);

        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<SphereCollider>().enabled = false;
        player.SetFallMultiplyer(basicFallForce);
        earthSkillActive = false;
        player.SetAnim("AirSkillEnd");
        player.SetIsStaticSkill(false);

        player.SetInSkill(false);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 dir = other.transform.position - this.transform.position + new Vector3(0, 2f, 0);
            other.gameObject.GetComponent<Rigidbody>().AddForce(dir * 1.5f, ForceMode.Impulse);

        }
    }
}
