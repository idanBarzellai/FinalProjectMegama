using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PowerUpButton : MonoBehaviour
{
    public PowerupsManager.PowerUpsPowers myPower;
    int powerupCost;
    public GameObject powerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        powerupCost = PowerupsManager.instance.PriceList[myPower];
        GetComponentInChildren<TMP_Text>().text = myPower.ToString() + "\n" + powerupCost.ToString() + "    ";
        
    }

    public void BuyPowerUp()
    {

        int currentCoinsOnPlayer = PlayerPrefs.GetInt("Coins");
        if (currentCoinsOnPlayer >= powerupCost)
        {
            PlayerPrefs.SetInt("Coins", currentCoinsOnPlayer - powerupCost);
            UIController.instance.TurnOnOffAllBuyButtons(myPower, true);
            powerPrefab.SetActive(false);
            GetComponentInChildren<TMP_Text>().text = "Bought!";
            PlayerSpawner.instance.addedPower = myPower;
        }
    }
}
