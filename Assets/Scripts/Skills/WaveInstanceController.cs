using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WaveInstanceController : SkillInstanceController
{
    float pushForce = 30f;

    protected override void Start()
    {
        base.Start();
        dmg = 5;
        skillManager.currectActiveSkillInstances.Add(this.gameObject);
        //StartCoroutine(skillManager.DestroyOvertime(lifetime, photonView.Owner));

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!playerName.IsNullOrEmpty())
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {

                Vector3 dir = transform.forward;
                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg, dir * pushForce, player.gameObject.GetPhotonView().Owner.ActorNumber,  playerName);
                //other.gameObject.GetPhotonView().RPC("PushedForce", RpcTarget.All, dir * pushForce);

            }
        }
    }
}
