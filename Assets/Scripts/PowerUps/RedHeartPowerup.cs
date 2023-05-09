using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHeartPowerup : PowerupBaseController
{
    int bonusLifePoints = 10;
    protected override void PowerupPowerHandler()
    {
        playerThatTookMe.ApplyPowerup(PowerupsManager.PowerUpsPowers.ExtraLife);
    }
}
