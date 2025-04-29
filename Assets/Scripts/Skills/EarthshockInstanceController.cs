using Photon.Pun;
using UnityEngine;

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
        if (!string.IsNullOrEmpty(playerName))
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {

                Vector3 dir = other.transform.position - this.transform.position + new Vector3(0, 2f, 0);
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 5);

                //other.gameObject.GetPhotonView().RPC("PushedForce", RpcTarget.All, dir * pushForce);
                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg, dir * pushForce, PhotonNetwork.LocalPlayer.ActorNumber, playerName);

            }
        }
    }
}
