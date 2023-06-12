using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPowerup : PowerupBaseController
{
    protected override void PowerupPowerHandler()
    {
        playerThatTookMe.ApplyPowerup(PowerupsManager.PowerUpsPowers.Armor);
    }
}
