using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAttackParticle : MonoBehaviour
{
    Creature targetedCreature;

    public void SetTarget(Creature creatureToTarget)
    {
        targetedCreature = creatureToTarget;
    }

    private void Update()
    {
        if (targetedCreature != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetedCreature.transform.position, 10f * Time.deltaTime);
        }
    }
}
