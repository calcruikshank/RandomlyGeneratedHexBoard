using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAbility : MonoBehaviour
{
    protected Creature thisCreature;
    protected int abilityUsageRate = 40;
    int abilityTimer;
    void Start()
    {
        thisCreature = this.GetComponent<Creature>();
    }

    private void FixedUpdate()
    {
        abilityTimer += 1;
        if (abilityTimer > abilityUsageRate)
        {
            abilityTimer = 0;
            UseAbility();
        }
    }

    protected virtual void UseAbility()
    {

    }
}
