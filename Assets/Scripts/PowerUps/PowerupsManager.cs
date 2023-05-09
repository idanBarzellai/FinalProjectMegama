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
        HigherJump,
        ExtraLife,
        Armor,
        ExtraDmg,
        Speed,
        CooldownReduction,
        Coin,
        Null
    }

    public Dictionary<PowerUpsPowers, int> PriceList = new Dictionary<PowerUpsPowers, int>()
    {
        {PowerUpsPowers.HigherJump, 50 },
        {PowerUpsPowers.ExtraLife, 50},
        {PowerUpsPowers.Armor, 100},
        {PowerUpsPowers.ExtraDmg, 150},
        {PowerUpsPowers.Speed, 50},
        {PowerUpsPowers.CooldownReduction, 150},

    };

    public Dictionary<PowerUpsPowers, int> AdditionList = new Dictionary<PowerUpsPowers, int>()
    {
        {PowerUpsPowers.HigherJump, 20 },
        {PowerUpsPowers.ExtraLife, 50},
        {PowerUpsPowers.Armor, 100},
        {PowerUpsPowers.ExtraDmg, 5},
        {PowerUpsPowers.Speed, 3},
        {PowerUpsPowers.CooldownReduction, 1},
        {PowerUpsPowers.Coin, 1}

    };

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
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
