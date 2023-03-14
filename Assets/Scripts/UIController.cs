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

    public void Respawn()
    {
        respawntext.gameObject.SetActive(false);
        deathText.text = "Respawing...";
    }
}
