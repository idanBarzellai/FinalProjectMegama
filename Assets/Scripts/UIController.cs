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

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ShowHideOptions();
        }

        if (optionsScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
