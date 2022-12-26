using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private EnemyStats enemyStats;
    private GameObject playerCamera;
    public Slider bar;

    void Start()
    {
        playerCamera = GameObject.Find("Camera");
        enemyStats = GetComponent<EnemyStats>();
        bar.maxValue = enemyStats.maxHP;
    }

    void Update()
    {
        bar.transform.LookAt(playerCamera.transform);
        bar.value = enemyStats.currHP;
    }
}
