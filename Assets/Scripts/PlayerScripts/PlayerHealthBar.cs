using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider bar;

    void Start()
    {
        bar.maxValue = PlayerStats.Inctance.maxHP;
    }

    void Update()
    {
        bar.value = PlayerStats.Inctance.currHP;
    }
}
