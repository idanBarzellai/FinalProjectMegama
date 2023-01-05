using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using WebSocketSharp;

public class FireInstanceController : SkillInstanceController
{

    float dpsCooldown = 0.5f;

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
                BasicsController player = other.GetComponent<BasicsController>();
                if (!player.GetReceivingDPS())
                {
                    player.SetReceivingDPS(true);
                    StartCoroutine(InflictDPS(player));
                }
                Debug.Log("Inflicting dps");
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (!playerName.IsNullOrEmpty())
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
                other.GetComponent<BasicsController>().SetReceivingDPS(false);

    }

    private IEnumerator InflictDPS(BasicsController player)
    {
        while (player.GetReceivingDPS())
        {
            GameObject hitfx = PhotonNetwork.Instantiate(hitEffect.name, player.gameObject.transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(hitfx);
            player.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg,Vector3.zero, playerName);
            yield return new WaitForSecondsRealtime(dpsCooldown);
        }
    }

}
