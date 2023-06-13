using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDecor : MonoBehaviour
{
    public GameObject[] decors;
    public int decorChance = 33;
    void Start()
    {
        if(Random.Range(0f, 1f) <= Mathf.Clamp((float)decorChance, 0f, 100f)  / 100f)
        {
            float scaleFactor = FindObjectOfType<Generate>().GetScaleFactor() / 2;
            Vector3 pos = transform.position + new Vector3(Random.Range(0, scaleFactor), scaleFactor, Random.Range(0, scaleFactor));
            GameObject decorToSpawn = decors[Random.Range(0, decors.Length)];
            //decorToSpawn.transform.localScale = decorToSpawn.transform.localScale * scaleFactor / 2;
            Instantiate(decorToSpawn, pos, Quaternion.identity, this.transform);
        }
        
    }

}
