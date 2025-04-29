using Photon.Pun;
using UnityEngine;

public class WaveInstanceController : SkillInstanceController
{
    float pushForce = 30f;
    Vector3 dir = Vector3.zero;
    public float waveSpeed = 2f;

    protected override void Start()
    {
        base.Start();
        dmg = 5;
        skillManager.currectActiveSkillInstances.Add(this.gameObject);
        //StartCoroutine(skillManager.DestroyOvertime(lifetime, photonView.Owner));

    }

    private void Update()
    {
        transform.Translate(dir * waveSpeed * Time.deltaTime);
    }

    public void Move(Vector3 _dir)
    {
        dir = _dir;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(playerName))
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 3);

                Vector3 dir = transform.forward;
                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, dmg, dir * pushForce, player.gameObject.GetPhotonView().Owner.ActorNumber, playerName);
                //other.gameObject.GetPhotonView().RPC("PushedForce", RpcTarget.All, dir * pushForce);

            }
        }
    }
}
