
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class EffectController : MonoBehaviourPunCallbacks
{
    public string playerName;
    float lifetime = 5f;

    private void Start()
    {
        StartCoroutine(DestroyOvertime(lifetime));
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
