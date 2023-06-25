
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    GameObject [] spawnPointsGameobjects;
    public List<Transform> spawnPoints;
    private void Awake()
    {
        instance= this;
        spawnPointsGameobjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        RestartRespawnList();
    }

    public static Transform GetSpawnPoint()
    {
        var spawnPoints = instance.spawnPoints;
        Debug.Log(spawnPoints == null ? (instance == null ? "instance is null" : "instance isnt null but spawn points list is null") 
                    : $"spawn points count: {spawnPoints.Count}");

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        spawnPoints.Remove(spawnPoint);
        return spawnPoint;
    }
        
    public static void RestartRespawnList() {
        var spawnPoints = instance.spawnPoints;
        var spawnPointsGameobjects = instance.spawnPointsGameobjects;
        
        spawnPoints = new List<Transform>();
        foreach (GameObject spawnPoint in spawnPointsGameobjects) spawnPoints.Add(spawnPoint.transform);
    }

    void SetAllActive(bool isWaiting) {foreach (GameObject spawnPoint in spawnPointsGameobjects) spawnPoint.SetActive(isWaiting);}
    bool lastIsWaiting;
    void Update()
    {
        bool isWaiting = MatchManager.GetState() == MatchManager.GameState.Waiting;

        if (isWaiting == lastIsWaiting) return;
        
        lastIsWaiting = isWaiting;
        SetAllActive(isWaiting);

        bool isPlaying = MatchManager.GetState() == MatchManager.GameState.Playing;
        if(isPlaying == false) RestartRespawnList(); 
    }
 
}
