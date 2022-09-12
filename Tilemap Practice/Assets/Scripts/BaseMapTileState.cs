using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMapTileState : MonoBehaviour
{
    public static BaseMapTileState singleton;

    public Dictionary<Vector3Int, BaseTile> baseTiles = new Dictionary<Vector3Int, BaseTile>();
    private void Awake()
    {
        if (singleton != null) Destroy(this);
        singleton = this;
    }


    internal bool GetCreatureAtTile(Vector3Int currentCellPosition)
    {
        return false;
    }

    internal void AddToBaseTiles(Vector3Int currentCellPosition, BaseTile baseTile)
    {
        if (!baseTiles.ContainsKey(currentCellPosition))
        {
            baseTiles.Add(currentCellPosition, baseTile);
        }
    }
}
