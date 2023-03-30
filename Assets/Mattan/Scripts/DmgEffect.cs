using UnityEngine;
using Photon.Pun;

public class DmgEffect : Effect {
    [SerializeField] int damage;


    public override void Apply(){
        Vector3 push = Vector3.zero;

        GetComponent<Tile>().otherPlayerPhotoneView.RPC("DealDamage", RpcTarget.All,
                                                         damage, push,
                                                        gameObject.name);

    }


}