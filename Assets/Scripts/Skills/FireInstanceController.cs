using System.Collections;
using UnityEngine;
using Photon.Pun;

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
        if (!string.IsNullOrEmpty(playerName))
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {
                BasicsController player = other.GetComponent<BasicsController>();
                if (!player.GetReceivingDPS())
                {
                    player.SetReceivingDPS(true);
                    StartCoroutine(InflictDPS(player));
                }
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (!string.IsNullOrEmpty(playerName))
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
                other.GetComponent<BasicsController>().SetReceivingDPS(false);

    }

    private IEnumerator InflictDPS(BasicsController player)
    {
        while (player.GetReceivingDPS())
        {
            GameObject hitfx = PhotonNetwork.Instantiate(hitEffect.name, player.gameObject.transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(hitfx);
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1);

            player.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg, Vector3.zero, player.gameObject.GetPhotonView().Owner.ActorNumber, playerName);
            yield return new WaitForSecondsRealtime(dpsCooldown);
        }
    }

}
