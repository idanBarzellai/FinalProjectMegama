using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance= this; 
    }
    public GameObject deathScreen;
    public TMP_Text deathText;
    public TMP_Text respawntext;


    public Slider healthSlider, skillSlider;
    public Image skillSliderFillColor;

    public TMP_Text killsText;
    public TMP_Text deathsText;


    public GameObject leaderboard;
    public LeaderboardPlayer leaderboardPlayerDisplay;

    public GameObject endScreen;

    public TMP_Text timerText;

    public GameObject optionsScreen;

    public TMP_Text currentCoins;
    public GameObject[] powerupsButtons;


    public enum PlayerChosen
    {
        Air, Fire, Water, Earth, None
    }

    public GameObject playerChoosingScreen;
    public Toggle airButton, fireButton, waterButton, earthButton;
    public PlayerChosen isPlayerPicked = PlayerChosen.None;

    private void Start()
    {
        ReleasePlayersChooseButtons();
        playerChoosingScreen.SetActive(true);

        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowHideOptions();
        }

        if (optionsScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        currentCoins.text =  PlayerPrefs.GetInt("Coins").ToString() + " Coins";
    }
    public void Respawn()
    {
        respawntext.gameObject.SetActive(false);
        deathText.text = "Respawing...";
    }

    public void ShowHideOptions()
    {

        optionsScreen.SetActive(optionsScreen.activeInHierarchy ? false : true);
        
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();

    }

    public void ChoosePlayerFire()
    {
        ReleasePlayersChooseButtons();
        fireButton.SetIsOnWithoutNotify(false);
        isPlayerPicked = PlayerChosen.Fire;

    }

    public void ChoosePlayerWater()
    {
        ReleasePlayersChooseButtons();
        waterButton.SetIsOnWithoutNotify(false);
        isPlayerPicked = PlayerChosen.Water;
    }
    public void ChoosePlayerEarth()
    {
        ReleasePlayersChooseButtons();
        earthButton.SetIsOnWithoutNotify(false);
        isPlayerPicked = PlayerChosen.Earth;
    }
    public void ChoosePlayerAir()
    {
        ReleasePlayersChooseButtons();
        airButton.SetIsOnWithoutNotify(false);
        isPlayerPicked = PlayerChosen.Air;
    }

    public void ReleasePlayersChooseButtons()
    {
        fireButton.SetIsOnWithoutNotify(true) ;
        waterButton.SetIsOnWithoutNotify(true);
        airButton.SetIsOnWithoutNotify(true);
        earthButton.SetIsOnWithoutNotify(true);
        isPlayerPicked = PlayerChosen.None;

    }

    public void ShowHidePlayerChoosingScreen()
    {
        playerChoosingScreen.SetActive(playerChoosingScreen.activeInHierarchy ? false : true);
    }

    public void StartMatchPlayerChosen()
    {
        PlayerSpawner.instance.SpawnPlayer();
    }

    
    public void TurnOffAllBuyButtons(PowerupsManager.PowerUpsPowers power)
    {
        foreach (GameObject button in powerupsButtons)
        {
            if (button.GetComponent<PowerUpButton>().myPower != power)
            {
                button.SetActive(false);
            }
            
        } 
    }

    public void DebugCoins()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 50);
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
        Application.Quit();
    }
}
