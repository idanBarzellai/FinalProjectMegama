using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using WebSocketSharp;

public class ShotController : MonoBehaviourPunCallbacks
{
    public GameObject hitEffect;
    public string playerName;
    int dmg = 10;
    float lifetime = 5f;

    private void Start()
    {
        StartCoroutine(DestroyOvertime(lifetime));
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!playerName.IsNullOrEmpty())
        {
            if (other.CompareTag("Player") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {
                PhotonNetwork.Instantiate(hitEffect.name, other.gameObject.transform.position, Quaternion.identity);
                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All,dmg ,playerName);
            }
        }

    }

    public void SetName(string name)
    {
        playerName = name;
    }

    public IEnumerator DestroyOvertime(float waitBeforeDestroy)
    {
        yield return new WaitForSecondsRealtime(waitBeforeDestroy);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

    }
}
