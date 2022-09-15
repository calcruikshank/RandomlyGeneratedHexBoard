using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseMapTileState : MonoBehaviour
{
    public static BaseMapTileState singleton;

    Tilemap baseMap;
    public Dictionary<Vector3Int, BaseTile> baseTiles = new Dictionary<Vector3Int, BaseTile>();
    private void Awake()
    {
        if (singleton != null) Destroy(this);
        singleton = this;
        baseMap = this.GetComponent<Tilemap>();
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
        if (baseMap.GetInstantiatedObject(currentCellPosition))
        {
            if (!baseTiles.ContainsKey(currentCellPosition))
            {
                baseTiles.Add(currentCellPosition, baseMap.GetInstantiatedObject(currentCellPosition).GetComponent<BaseTile>());
            }
            //has a non water tile
        }
        else
        {
            //is a water tile
            baseTiles.Add(currentCellPosition, baseTile);
        }
    }

    internal BaseTile GetBaseTileAtCellPosition(Vector3Int currentCellPosition)
    {
        if (baseTiles.ContainsKey(currentCellPosition))
        {
            BaseTile baseTile = baseTiles[currentCellPosition];
            return baseTile;
        }
        else
        {
            return null;
        }
    }

}
