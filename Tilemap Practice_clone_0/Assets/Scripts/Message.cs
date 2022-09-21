using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Message 
{
    public List<Vector3> leftClicksWorldPos = new List<Vector3>();
    public List<int> guidsForCards = new List<int>();
}
