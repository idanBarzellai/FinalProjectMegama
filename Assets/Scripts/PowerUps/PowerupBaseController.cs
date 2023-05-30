using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PowerupBaseController : MonoBehaviourPunCallbacks
{
    protected float createdTime;
    protected float timeForDestruction = 25;
    protected float timeForDestrcutionIfTouced = 0.5f;
    public ParticleSystem disappearEffect;
    protected BasicsController playerThatTookMe;
    /*
              
              * apply object on player
              * add player UI element
              */
    protected PowerupsManager powerupsManager;
    protected virtual void Start()
    {
        powerupsManager = PowerupsManager.instance;
        powerupsManager.DestoryOverNetwork(timeForDestruction, this.gameObject);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerThatTookMe = other.GetComponent<BasicsController>();
            PowerupPowerHandler();
            disappearEffect.Play();
            powerupsManager.DestoryOverNetwork(0, this.gameObject);
        }
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Destroy(rb);
        }
    }

    protected virtual void PowerupPowerHandler()
    {
        Debug.Log("Not implemented");
    }
}
