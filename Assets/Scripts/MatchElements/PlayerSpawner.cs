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
    public PowerupsManager.PowerUpsPowers addedPower = PowerupsManager.PowerUpsPowers.Null;

    public void SpawnPlayer()
    {
        if (PhotonNetwork.IsConnected) { 
            UIController.instance.playerChoosingScreen.SetActive(false);

            // UIController.instance.skillOrb.gameObject.SetActive(true);
            UIController.instance.skillSlider.gameObject.SetActive(true);
            // UIController.instance.healthOrb.gameObject.SetActive(true);
            UIController.instance.healthSlider.gameObject.SetActive(true);
            
            Transform spawnPoint = SpawnManager.GetSpawnPoint();

            int playerChosen = (int)UIController.instance.isPlayerPicked == playerPrefabs.Length ? 
                                Random.Range(0, playerPrefabs.Length) :
                                (int)UIController.instance.isPlayerPicked;
            // int randomPlayer = playerChosen == 4 ?  : ;
            GameObject playerToSpawn = playerPrefabs[playerChosen];
            player = PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, spawnPoint.rotation);
            UIController.instance.PickImage(playerChosen); 

            UIController.instance.androidUI.SetController(player.GetComponent<BasicsController>());
            

            // Addd powerup
            if(addedPower != PowerupsManager.PowerUpsPowers.Null)
            {
                player.GetComponent<BasicsController>().ApplyPowerup(addedPower);
            }
        }
    }

    public void Die(string killingPlayer)
    {
        MatchManager.instance.UpdateStatSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
        UIController.instance.deathText.text = "You were killed by " + killingPlayer;
        UIController.instance.respawntext.gameObject.SetActive(true);
        UIController.instance.deathScreen.SetActive(true);

        UIController.instance.skillSlider.gameObject.SetActive(false);
        UIController.instance.healthSlider.gameObject.SetActive(false);

        addedPower = PowerupsManager.PowerUpsPowers.Null;
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
            player = null;
        }

        
        UIController.instance.Respawn();

        yield return new WaitForSecondsRealtime(respawnTime);

        UIController.instance.deathScreen.SetActive(false);

        if(MatchManager.instance.state == MatchManager.GameState.Playing && player == null)
        {
            SpawnPlayer();
        }

    }


}
