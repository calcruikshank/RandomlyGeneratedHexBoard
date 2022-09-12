using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    Transform environmentTileOnBaseTile;
    Vector3Int tilePosition;
    Grid grid;
    Creature creatureOnTile;
    
    private void Start()
    {
        grid = GameManager.singleton.grid;
        Vector3Int currentCellPosition = grid.WorldToCell(this.transform.position);
        BaseMapTileState.singleton.AddToBaseTiles(currentCellPosition,this);
    }

    internal Creature CreatureOnTile()
    {
        return creatureOnTile;
    }

    internal void AddCreatureToTile(Creature creature)
    {
        creatureOnTile = creature;
    }
}
