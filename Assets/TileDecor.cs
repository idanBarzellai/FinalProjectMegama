using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDecor : MonoBehaviour
{
    public GameObject[] decors;
    void Start()
    {
        if(Random.Range(0, 2) == 1)
        {
            float scaleFactor = FindObjectOfType<generate>().GetScaleFactor();
            Vector3 pos = transform.position + new Vector3(Random.Range(0, scaleFactor / 2), scaleFactor, Random.Range(0, scaleFactor / 2));
            GameObject decorToSpawn = decors[Random.Range(0, decors.Length)];
            //decorToSpawn.transform.localScale = decorToSpawn.transform.localScale * scaleFactor / 2;
            Instantiate(decorToSpawn, pos, Quaternion.identity, this.transform);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
