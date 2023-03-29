using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupsManager : MonoBehaviourPunCallbacks
{
    public static PowerupsManager instance;
    //public List<PowerupBaseController> currectActivePowerups = new List<PowerupBaseController>();
    public GameObject[] powerupsToSummon;
    public GameObject[] spawnPoints;
    //bool looping = false;

    public enum PowerUpsPowers
    {
        DoubleJump,
        ExtraLife,
        Shield,
        Armor,
        ExtraDmg,
        Speed
    }
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(powerupsToSummon[Random.Range(0, powerupsToSummon.Length)].name, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void DestoryOverNetwork(float waitBeforeDestroy, GameObject obj)
    {
        StartCoroutine(DestroyOvertime(waitBeforeDestroy, obj));
    }
    public IEnumerator DestroyOvertime(float waitBeforeDestroy, GameObject obj)
    {
        yield return new WaitForSecondsRealtime(waitBeforeDestroy);
        if (obj)
        {
            // effect for destruction
            PhotonNetwork.Destroy(obj);
        }
    }
}
