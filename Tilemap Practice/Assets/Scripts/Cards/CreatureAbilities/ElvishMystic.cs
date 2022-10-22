using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElvishMystic : CreatureAbility
{
    protected override void UseAbility()
    {
        if (thisCreature.playerOwningCreature.tilesOwned.ContainsValue( thisCreature.tileCurrentlyOn ))
        {
            thisCreature.playerOwningCreature.AddSpecificManaToPool(thisCreature.tileCurrentlyOn.manaType);
        }
    }
}
