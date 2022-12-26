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

    public Slider healthSlider;
}
