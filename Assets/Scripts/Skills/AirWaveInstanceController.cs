using Photon.Pun;
using UnityEngine;

public class AirWaveInstanceController : SkillInstanceController
{
    float pushForce = 15f;
    public float distPower = 10;

    protected override void Start()
    {
        base.Start();

        dmg = 15;

    }
    private void OnTriggerEnter(Collider other)
    {

        if (!string.IsNullOrEmpty(playerName))
        {
            PhotonView otherPlayerPhotoneView = other.gameObject.GetPhotonView();
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {
                float dist = Vector3.Distance(other.transform.position, this.transform.position);

                Vector3 dir = other.transform.position - this.transform.position;
                Vector3 push = (dir.normalized * (distPower / dist) * pushForce);
                push.y = 2;
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 5);

                //otherPlayerPhotoneView.RPC("PushedForce", RpcTarget.All,push);
                otherPlayerPhotoneView.RPC("DealDamage", RpcTarget.All, dmg, push, PhotonNetwork.LocalPlayer.ActorNumber, playerName);

            }
        }
    }

}
