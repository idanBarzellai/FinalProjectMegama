using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpPowerup : PowerupBaseController
{
    int jumpIncreseAmount = 20;
    protected override void PowerupPowerHandler()
    {
        playerThatTookMe.ApplyPowerup(PowerupsManager.PowerUpsPowers.HigherJump);
    }
}
