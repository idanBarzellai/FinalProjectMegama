using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using WebSocketSharp;

public class FireInstanceController : SkillInstanceController
{
    protected override void Start()
    {
        base.Start();
        dmg = 5;
        skillManager.currectActiveSkillInstances.Add(this.gameObject);
        StartCoroutine(skillManager.DestroyOvertime(lifetime));
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!playerName.IsNullOrEmpty())
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {

                PhotonNetwork.Instantiate(hitEffect.name, other.gameObject.transform.position, Quaternion.identity);
                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg, photonView.name);

                Debug.Log("Inflicting dps");
            }
        }

    }

}
