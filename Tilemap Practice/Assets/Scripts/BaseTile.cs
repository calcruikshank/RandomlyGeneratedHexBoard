using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    Transform environmentTileOnBaseTile;
    Vector3Int tilePosition;
    Grid grid;
    private void Awake()
    {

    }
    private void Start()
    {
        grid = GameManager.singleton.grid;
        Vector3Int currentCellPosition = grid.WorldToCell(this.transform.position);
        BaseMapTileState.singleton.AddToBaseTiles(currentCellPosition,this);
        Debug.Log(currentCellPosition);
    }


}
