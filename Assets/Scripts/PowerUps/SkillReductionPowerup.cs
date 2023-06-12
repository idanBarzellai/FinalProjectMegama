using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillReductionPowerup : PowerupBaseController
{
    protected override void PowerupPowerHandler()
    {
        playerThatTookMe.ApplyPowerup(PowerupsManager.PowerUpsPowers.ShortCooldown);
    }
}
