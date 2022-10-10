using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding
{
   Grid grid;
   Tilemap baseMap;
    private List<BaseTile> openList;
    private List<BaseTile> closedList;
    public List<BaseTile> FindPath(Vector3Int startingPosition, Vector3Int endingPosition)
    {
        Debug.Log("starting and ending  = +" + startingPosition + " " + endingPosition);
        openList = new List<BaseTile>();
        BaseTile startingTile = BaseMapTileState.singleton.GetBaseTileAtCellPosition(startingPosition);
        openList = new List<BaseTile> { startingTile };
        closedList = new List<BaseTile>();

        Debug.Log(GameManager.singleton.startingX + " startying x " );
        for (int x = GameManager.singleton.startingX; x < GameManager.singleton.endingX; x++)
        {
            for (int y = GameManager.singleton.startingY; y < GameManager.singleton.endingY; y++) 
            {
                BaseTile baseTile = BaseMapTileState.singleton.GetBaseTileAtCellPosition(new Vector3Int(x, y, 0));
                //baseTile.gameObject.SetActive(false);
            }

        }
        return null;
    }
}
