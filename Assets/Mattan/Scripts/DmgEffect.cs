using UnityEngine;
using Photon.Pun;

public class DmgEffect : Effect {
    [SerializeField] int damage;
    Tile tile;

    private void Start()
    {
        tile = GetComponent<Tile>();
    }

    public override void Apply(){
        Vector3 push = Vector3.zero;

        if(tile.otherPlayerPhotoneView != null)
        {
            if (tile.otherPlayerPhotoneView.IsDead())
               tile.otherPlayerPhotoneView = null;
            else
                tile.otherPlayerPhotoneView.TakeDamage(damage, push, 200, "stepping on a bad tile!");
        }
        //GetComponent<Tile>().otherPlayerPhotoneView.RPC("DealDamage", RpcTarget.All,gameObject.name);

    }


}