using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenSpawner : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    public GameObject[] bodyParts;
    public float summonrate = 0.5f;

    private List<GameObject> spawnList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("SpawnParts",0, summonrate);
    }

    

    private void SpawnParts()
    {
        if (spawnList.Count <= 19)
        {
            GameObject bodypart = PhotonNetwork.Instantiate(bodyParts[Random.Range(0, bodyParts.Length)].name, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
            spawnList.Add(bodypart);
            Destroy(bodypart, 5f);
        }
    }
}
