using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPowerup : PowerupBaseController
{
    int attackIncreaseAmount = 10;
    protected override void PowerupPowerHandler()
    {
        playerThatTookMe.ApplyPowerup(PowerupsManager.PowerUpsPowers.ExtraDmg, attackIncreaseAmount);
    }
}
