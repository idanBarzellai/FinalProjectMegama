using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class EarthshockInstanceController : SkillInstanceController
{
    float pushForce = 6f;

    protected override void Start()
    {
        base.Start();

        lifetime = 0.5f;
        dmg = 30;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!playerName.IsNullOrEmpty())
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {

                Vector3 dir = other.transform.position - this.transform.position + new Vector3(0, 2f, 0);

                other.gameObject.GetPhotonView().RPC("PushedForce", RpcTarget.All, dir * pushForce);
                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg, photonView.name);

            }
        }
    }
}
