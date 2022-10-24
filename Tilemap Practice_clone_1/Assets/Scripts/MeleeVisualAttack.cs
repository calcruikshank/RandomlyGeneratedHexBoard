using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeVisualAttack : MonoBehaviour
{
    Creature targetedCreature;
    float amountofdamage;
    public bool shutDown = false;
    public void SetTarget(Creature creatureToTarget, float attack)
    {
        targetedCreature = creatureToTarget;
        amountofdamage = attack;
        this.GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shutDown)
        {
            return;
        }
        if (targetedCreature != null)
        {
            if (shutDown == false)
            {
                targetedCreature.TakeDamage(amountofdamage);
                this.GetComponent<ParticleSystem>().Stop();
                shutDown = true;
            }
        }
        if (targetedCreature == null)
        {
            this.GetComponent<ParticleSystem>().Stop();
            shutDown = true;
        }
    }
}
