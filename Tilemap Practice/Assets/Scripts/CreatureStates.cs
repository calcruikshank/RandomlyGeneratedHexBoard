using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStates : MonoBehaviour
{
    public State creatureState;
    float speed;
    float UsageRate; // the rate at which the minion can use abilities/ attack 

    private void Awake()
    {
        creatureState = State.Summoned;
    }
    public enum State
    {
        Summoned, //On The turn created
        Attack,
        UseAbility,
        Moving
        //not sure if i need a tapped state yet trying to keep it as simple as possible
    }
}
