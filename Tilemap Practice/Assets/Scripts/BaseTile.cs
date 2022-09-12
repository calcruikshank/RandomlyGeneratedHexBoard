using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTile : MonoBehaviour
{
    Transform environmentTileOnBaseTile;
    Vector3Int tilePosition;
    Grid grid;
    Tilemap environmentMap;
    Creature creatureOnTile;
    GameObject environmentOnTile;

    private void Start()
    {
        grid = GameManager.singleton.grid;
        environmentMap = GameManager.singleton.enviornmentMap;
        Vector3Int currentCellPosition = grid.WorldToCell(this.transform.position);
        BaseMapTileState.singleton.AddToBaseTiles(currentCellPosition,this);
        environmentOnTile = environmentMap.GetInstantiatedObject(currentCellPosition);
    }

    internal Creature CreatureOnTile()
    {
        return creatureOnTile;
    }

    internal void AddCreatureToTile(Creature creature)
    {
        creatureOnTile = creature;
        if (environmentOnTile != null)
        {
            if (environmentOnTile.GetComponent<ChangeTransparency>() == null)
            {
                environmentOnTile.AddComponent<ChangeTransparency>().ChangeTransparent(100);
            }
            else
            {
                environmentOnTile.GetComponent<ChangeTransparency>().ChangeTransparent(100);
            }
        }
       
    }
    internal void RemoveCreatureFromTile(Creature creature)
    {
        creatureOnTile = null;
        if (environmentOnTile != null)
        {
            if (environmentOnTile.GetComponent<ChangeTransparency>() == null)
            {
                environmentOnTile.AddComponent<ChangeTransparency>().SetOpaque();
            }
            else
            {
                environmentOnTile.GetComponent<ChangeTransparency>().SetOpaque();
            }
        }
    }
}
