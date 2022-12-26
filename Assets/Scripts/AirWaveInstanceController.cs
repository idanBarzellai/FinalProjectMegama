using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class AirWaveInstanceController : SkillInstanceController
{
    float pushForce = 30f;

    protected override void Start()
    {
        base.Start();

        dmg = 15;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (!playerName.IsNullOrEmpty())
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {

                Vector3 dir = other.transform.position - this.transform.position;
                other.gameObject.GetPhotonView().RPC("PushedForce", RpcTarget.All, dir * pushForce);
                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg, photonView.name);

            }
        }
    }
}
