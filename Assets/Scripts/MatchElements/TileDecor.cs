using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileDecor : MonoBehaviourPunCallbacks
{
    int decorChance = 33;
    public GameObject[] decors;
    void Start()
    {
        if (Random.Range(0f, 1f) <= Mathf.Clamp((float)decorChance, 0f, 100f) / 100f)
        //if(Random.Range(0, 3) == 1)
        {
            if (decors.Length != 0)
            {
                float scaleFactor = FindObjectOfType<Generate>().GetScaleFactor() / 2;
                Vector3 pos = transform.position + new Vector3(Random.Range(0, scaleFactor), scaleFactor, Random.Range(0, scaleFactor));
                GameObject decorToSpawn = decors[Random.Range(0, decors.Length)];
                //decorToSpawn.transform.localScale = decorToSpawn.transform.localScale * scaleFactor / 2;

                // TODO make this objetc child of the tile
                GameObject decor =  PhotonNetwork.Instantiate(decorToSpawn.name, pos, Quaternion.identity);
                decor.transform.parent = this.transform;


            }
        }
        
    }

}
