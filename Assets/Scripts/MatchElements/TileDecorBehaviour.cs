using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TileDecorBehaviour : MonoBehaviourPunCallbacks
{
    private void Awake() {
        inTestMode = FindObjectOfType<Generate>().inTestMode;
    }
    int decorChance = 40;
    public GameObject[] decors;
    bool inTestMode;
    object [] ParentData(){
        int parentViewID = GetComponent<PhotonView>().ViewID;
        object [] data = new object[2];
        data[1] = parentViewID;
        return data;
    }
    public void DecorTesting()
    {
        if (!inTestMode) return;

        if (Random.Range(0f, 1f) <= Mathf.Clamp((float)decorChance, 0f, 100f) / 100f)
        {
            if (decors.Length != 0)
            {
                GameObject decorToSpawn = decors[Random.Range(0, decors.Length)];
                float randomX = Random.Range(-0.6f, 0.2f);
                float randomZ = Random.Range(-0.6f, 0.2f);
                Vector3 pos = new Vector3(randomX, 0.75f + decorToSpawn.transform.localPosition.y, randomZ);


                string decorPath = $"Tiles/Decors/{decorToSpawn.name}";
                GameObject decor = Instantiate(Resources.Load(decorPath), pos, Quaternion.identity) as GameObject;      
                decor.transform.SetParent(this.transform, false);
            }
        }
    }

    public void Decor()
    {
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
                                    :   PhotonNetwork.Instantiate(decorPath, pos, Quaternion.identity, 0, ParentData());
            }
        }
    }

    public void SpawnBehaviourTesting (string pathToBehaviourToSpawn)
    {
        if (!inTestMode) return;
        GameObject behaviour = Instantiate(Resources.Load(pathToBehaviourToSpawn), 
                                        Vector3.zero, 
                                        Quaternion.Euler(0, 0, 0)) as GameObject;   
        behaviour.transform.SetParent(this.transform, false);
    }

    public void SpawnBehaviour(string pathToBehaviourToSpawn)
    {
        GameObject behaviour = inTestMode 
                                ?   Instantiate(Resources.Load(pathToBehaviourToSpawn), 
                                        Vector3.zero, 
                                        Quaternion.Euler(0, 0, 0)) as GameObject
                                :   PhotonNetwork.Instantiate(
                                    pathToBehaviourToSpawn, 
                                    Vector3.zero, 
                                    Quaternion.Euler(0, 0, 0), 0, ParentData());   
    }
}
