using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private float lookRadius = 8f;
    private Vector3 lookVector;
    private bool playerDetected;
    private bool inAttakDis;
    private bool isAttaking;
    private Transform target;
    private NavMeshAgent navMeshAgent;
    private float nextAttakAt = 0;
    private EnemyStats enemyStats;

    public Animator pawnBlackAnimator;
    public Animator PawnBlackAttakAnimation;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = enemyStats.speed;
        target = PlayerStats.Inctance.transform;
        playerDetected = false;
        inAttakDis = false;
        isAttaking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) <= lookRadius)
        {
            playerDetected = true;
        }

        if (Vector3.Distance(transform.position, target.position) <= navMeshAgent.stoppingDistance) inAttakDis = true;
        else inAttakDis = false;

        if (playerDetected)
        {

            if (!isAttaking)
            {
                lookVector = PlayerStats.Inctance.transform.position - transform.position;
                lookVector.y = 0f;
                transform.rotation = Quaternion.LookRotation(lookVector);

                if (!inAttakDis)
                {
                    pawnBlackAnimator.ResetTrigger("Idle");
                    pawnBlackAnimator.SetTrigger("Run");
                    navMeshAgent.SetDestination(target.position);
                    isAttaking = false;

                }
                else
                {
                    pawnBlackAnimator.ResetTrigger("Run");
                    pawnBlackAnimator.SetTrigger("Idle");
                    if (!isAttaking && inAttakDis) StartCoroutine(getRdyForAttak());
                }
            }  
        }

        IEnumerator getRdyForAttak()
        {
            if (nextAttakAt <= Time.time)
            {
                isAttaking = true;
                //Activate attak alart
                yield return new WaitForSeconds(1f);
                pawnBlackAnimator.Play("Attak");
                PawnBlackAttakAnimation.Play("PawnBlackAttakAnimation_Attak");
                nextAttakAt = Time.time + (1 / 0.5f);
                yield return new WaitForSeconds(pawnBlackAnimator.GetCurrentAnimatorStateInfo(0).length);
                isAttaking = false;
            }
        }

        /* private void OnDrawGizmosSelected()
         {
             Gizmos.color = Color.red;
             Gizmos.DrawWireSphere(transform.position, lookRadius);
         }
         */
    }
}
