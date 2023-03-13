using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttakCollisionCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyStats>().TakeDmg(PlayerStats.Inctance.dmg);
        }
    }
}
