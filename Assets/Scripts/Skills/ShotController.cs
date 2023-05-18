using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using WebSocketSharp;

public class ShotController : MonoBehaviourPunCallbacks
{
    public GameObject hitEffect;
    public GameObject coinObject;
    private GameObject coinToss;

    private Vector3 offset = new Vector3(0, 1.5f, 0);
    private Vector3 coinoffset = new Vector3(0, 1.5f, 0);

    public string playerName;
    public BasicsController shooter;
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
            if (other.CompareTag("Player") && !other.CompareTag("DeadHead") && other.gameObject.GetPhotonView().Owner.NickName != playerName)
            {
                PhotonNetwork.Instantiate(hitEffect.name, other.gameObject.transform.position + offset, Quaternion.identity);
                coinToss = Instantiate(coinObject, other.gameObject.transform.position + offset, Quaternion.identity);
                coinToss.GetComponentInChildren<Coin>().SetPlayer(shooter);

                other.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All,dmg , Vector3.zero, PhotonNetwork.LocalPlayer.ActorNumber, playerName);
                PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1);
            }
        }

    }

    public void SetName(string name)
    {
        playerName = name;
    }

    public void SetPlayer(BasicsController _player)
    {
        shooter = _player;
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
