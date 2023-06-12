using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupsManager : MonoBehaviourPunCallbacks
{
    public static PowerupsManager instance;
    //public List<PowerupBaseController> currectActivePowerups = new List<PowerupBaseController>();
    public GameObject[] powerupsToSummon;
    public Transform spawnPointParent;
    float spawnDelay = 20f;
    float spawnStart= 2f;
    float powerUpYOffset = 25;

    //bool looping = false;

    public enum PowerUpsPowers
    {
        HigherJump,
        ExtraLife,
        Armor,
        ExtraDmg,
        Speed,
        ShortCooldown,
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
        {PowerUpsPowers.ShortCooldown, 150},

    };

    public Dictionary<PowerUpsPowers, int> AdditionList = new Dictionary<PowerUpsPowers, int>()
    {
        {PowerUpsPowers.HigherJump, 20 },
        {PowerUpsPowers.ExtraLife, 50},
        {PowerUpsPowers.Armor, 100},
        {PowerUpsPowers.ExtraDmg, 5},
        {PowerUpsPowers.Speed, 3},
        {PowerUpsPowers.ShortCooldown, 1},
        {PowerUpsPowers.Coin, 1}

    };

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InvokeRepeating("CreatePowerUp", spawnStart, spawnDelay);
            }
    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePowerUp()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Transform spawnPoint = spawnPointParent.GetChild(Random.Range(0, spawnPointParent.childCount));
            Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + powerUpYOffset, spawnPoint.position.z);
            PhotonNetwork.Instantiate(powerupsToSummon[Random.Range(0, powerupsToSummon.Length)].name, spawnPos, Quaternion.identity);
        }

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
