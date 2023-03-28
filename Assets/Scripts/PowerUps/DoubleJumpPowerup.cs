using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpPowerup : PowerupBaseController
{
    int jumpIncreseAmount = 5;
    protected override void PowerupPowerHandler()
    {
        playerThatTookMe.ApplyPowerup(PowerupsManager.PowerUpsPowers.DoubleJump, jumpIncreseAmount);
    }
}
