using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour {
    public Effect effect{get; private set;}
    public bool playerTouching{get; private set;}
    //public PhotonView otherPlayerPhotoneView{get; private set;}
    public BasicsController otherPlayerPhotoneView{get; set;}

    public virtual void Awake() {
        playerTouching = false;
        effect = GetComponent<Effect>();
    }

    private void OnCollisionEnter(Collision other) {        
        if (other.gameObject.GetComponent<BasicsController>() == null)
            return;

        //otherPlayerPhotoneView = other.gameObject.GetPhotonView();
        otherPlayerPhotoneView = other.gameObject.GetComponent<BasicsController>();
        playerTouching = true;
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.GetComponent<BasicsController>() == null)
            return;

        playerTouching = false;
        
        otherPlayerPhotoneView = null;
    }
}