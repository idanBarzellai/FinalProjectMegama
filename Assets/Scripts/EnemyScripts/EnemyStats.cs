using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float maxHP = 40f;
    public float currHP = 40f;
    public float speed = 3.5f;
    public float armor = 0f;
    public float dmg = 3f;


    void start()
    {
        currHP = maxHP;
    }

    public void TakeDmg(float playerDmg)
    {
        currHP -= playerDmg;
    }

    void Update()
    {
        if (currHP <= 0f)
        {
            Destroy(gameObject);
        }
    }

}
