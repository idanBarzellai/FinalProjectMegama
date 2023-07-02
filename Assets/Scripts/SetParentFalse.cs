using UnityEngine;
using Photon.Pun;

public class SetParentFalse : MonoBehaviourPun, IPunInstantiateMagicCallback {
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        GameObject parent = PhotonView.Find((int)instantiationData[1]).gameObject;
        this.transform.SetParent(parent.transform, false);
    }
}