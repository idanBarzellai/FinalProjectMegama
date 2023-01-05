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
        UIController.instance.skillSlider.gameObject.SetActive(true);
        UIController.instance.healthSlider.gameObject.SetActive(true);
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        GameObject playerToSpawn = playerPrefabs[Random.Range(0, playerPrefabs.Length)];
        player =  PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, spawnPoint.rotation);
    }

    public void Die(string killingPlayer)
    {

        UIController.instance.deathText.text = "You were killed by " + killingPlayer;
        UIController.instance.respawntext.gameObject.SetActive(true);
        UIController.instance.deathScreen.SetActive(true);
        UIController.instance.skillSlider.gameObject.SetActive(false);
        UIController.instance.healthSlider.gameObject.SetActive(false);
    }

    public void ReSpawn()
    {
        StartCoroutine(SpawnCo());
    }

    public IEnumerator SpawnCo()
    {
        if (player)
        {
            // Make sure all instances of instatieted objects are destoried

            PhotonNetwork.Destroy(player);
        }
        UIController.instance.Respawn();

        yield return new WaitForSecondsRealtime(respawnTime);

        UIController.instance.deathScreen.SetActive(false);
        SpawnPlayer();

    }


}
