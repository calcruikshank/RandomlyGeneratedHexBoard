using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Message 
{
    enum order
    {
        cardSelection,
        leftClick,
        creatureSelect
    }
    public List<Vector3> leftClicksWorldPos = new List<Vector3>();
    public List<int> guidsForCards = new List<int>();
    public List<int> guidsForCreatures = new List<int>();
}
