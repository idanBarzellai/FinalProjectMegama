using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileDecor : MonoBehaviourPunCallbacks
{
    int decorChance = 40;
    public GameObject[] decors;
    void Start()
    {
        if (Random.Range(0f, 1f) <= Mathf.Clamp((float)decorChance, 0f, 100f) / 100f)
        {
            if (decors.Length != 0)
            {
                GameObject decorToSpawn = decors[Random.Range(0, decors.Length)];
                float randomX = Random.Range(-2f, 0.2f);
                float randomZ = Random.Range(-2f, 0.2f);
                Vector3 pos = new Vector3(randomX, 0.75f + decorToSpawn.transform.localPosition.y, randomZ);


                string decorPath = $"Tiles/Decors/{decorToSpawn.name}";
                GameObject decor =  PhotonNetwork.Instantiate(decorPath, pos, Quaternion.identity);

                decor.transform.SetParent(this.transform, false);
                decor.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
            }
        }
        
    }

}
