using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttakCollisionCheck : MonoBehaviour
{
    public EnemyStats enemyStats;
    private PlayerStats playerStats;

    void Start()
    {

        //playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag== "Player")
        {
            PlayerStats.Inctance.takeDmg(enemyStats.dmg);
            //playerStats.takeDmg(enemyStats.dmg);
        }
    }
}
