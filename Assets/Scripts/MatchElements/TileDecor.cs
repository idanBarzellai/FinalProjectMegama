using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileDecor : MonoBehaviourPunCallbacks
{
    int decorChance = 40;
    public GameObject[] decors;
    private void Start() {
        bool inTestMode = MatchManager.GetState() == MatchManager.GameState.Testing;
        if (inTestMode) return;
        Decor();
    }
    public void Decor()
    {
        bool inTestMode = MatchManager.GetState() == MatchManager.GameState.Testing;

        if (Random.Range(0f, 1f) <= Mathf.Clamp((float)decorChance, 0f, 100f) / 100f)
        {
            if (decors.Length != 0)
            {
                GameObject decorToSpawn = decors[Random.Range(0, decors.Length)];
                float randomX = Random.Range(-0.6f, 0.2f);
                float randomZ = Random.Range(-0.6f, 0.2f);
                Vector3 pos = new Vector3(randomX, 0.75f + decorToSpawn.transform.localPosition.y, randomZ);


                string decorPath = $"Tiles/Decors/{decorToSpawn.name}";
                GameObject decor = inTestMode
                                    ?   Instantiate(Resources.Load(decorPath), pos, Quaternion.identity) as GameObject      
                                    :   PhotonNetwork.Instantiate(decorPath, pos, Quaternion.identity);

                decor.transform.SetParent(this.transform, false);
                decor.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
            }
        }
        
    }

}
