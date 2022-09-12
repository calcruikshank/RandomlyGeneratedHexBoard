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
    
    public Controller GetOwnerOfTile(Vector3Int cellPosition)
    {
        BaseTile baseTile = GetBaseTileAtCellPosition(cellPosition);
        return baseTile.playerOwningTile;
    }

    internal Creature GetCreatureAtTile(Vector3Int cellPosition)
    {
        BaseTile baseTile = GetBaseTileAtCellPosition(cellPosition);
        return baseTile.CreatureOnTile();
    }

    internal void AddToBaseTiles(Vector3Int currentCellPosition, BaseTile baseTile)
    {
        if (!baseTiles.ContainsKey(currentCellPosition))
        {
            baseTiles.Add(currentCellPosition, baseTile);
            
        }
    }

    internal BaseTile GetBaseTileAtCellPosition(Vector3Int currentCellPosition)
    {
        BaseTile baseTile = baseTiles[currentCellPosition];
        return baseTile;
    }

}
