using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    private void Awake()
    {
        instance= this;
    }

    public GameObject[] playerPrefabs;
    private GameObject player;
    private float respawnTime = 1.5f;
  
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        GameObject playerToSpawn = playerPrefabs[Random.Range(0, playerPrefabs.Length)];
        player =  PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, spawnPoint.rotation);
    }

    public void Die(string killingPlayer)
    {

        UIController.instance.deathText.text = "You were killed by " + killingPlayer;


        if (player)
        {
            StartCoroutine(DieCo());
        }
    }

    public IEnumerator DieCo()
    {
        PhotonNetwork.Destroy(player);
        UIController.instance.deathScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(respawnTime);

        UIController.instance.deathScreen.SetActive(false);

        SpawnPlayer();


    }
}
