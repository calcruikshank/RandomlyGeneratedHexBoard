using System;
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
        openList = new List<BaseTile>();
        BaseTile startingTile = BaseMapTileState.singleton.GetBaseTileAtCellPosition(startingPosition);
        BaseTile endingTile = BaseMapTileState.singleton.GetBaseTileAtCellPosition(endingPosition);
        openList = new List<BaseTile> { startingTile };
        closedList = new List<BaseTile>();

        for (int x = GameManager.singleton.startingX; x < GameManager.singleton.endingX; x++)
        {
            for (int y = GameManager.singleton.startingY; y < GameManager.singleton.endingY; y++) 
            {
                BaseTile baseTile = BaseMapTileState.singleton.GetBaseTileAtCellPosition(new Vector3Int(x, y, 0));
                //baseTile.gameObject.SetActive(false);
                baseTile.gCost = int.MaxValue;
                baseTile.CalculateFCost();
                baseTile.cameFromBaseTile = null;
            }

        }

        startingTile.gCost = 0;
        startingTile.hCost = CalculateDistanceCost(startingTile, endingTile);
        startingTile.CalculateFCost();

        while (openList.Count > 0)
        {
            BaseTile currentTile = GetTheLowestFCostNode(openList);

            if (currentTile == endingTile)
            {
                return CalculatePath(endingTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (BaseTile neighbor in currentTile.neighborTiles)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                int tentativeGCost = currentTile.gCost + CalculateDistanceCost(currentTile, neighbor);
                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.cameFromBaseTile = currentTile;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateDistanceCost(neighbor, endingTile);
                    neighbor.CalculateFCost();

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }

        }


        return null;
    }

    List<BaseTile> CalculatePath(BaseTile endingTileSent)
    {
        List<BaseTile> path = new List<BaseTile>();
        path.Add(endingTileSent);
        BaseTile currentNode = endingTileSent;

        while (currentNode.cameFromBaseTile != null)
        {
            path.Add(currentNode.cameFromBaseTile);
            currentNode = currentNode.cameFromBaseTile;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(BaseTile a, BaseTile b)
    {
        int xDistance = Mathf.Abs(Mathf.Abs(a.tilePosition.x) - Mathf.Abs(b.tilePosition.x));
        int yDistance = Mathf.Abs(Mathf.Abs(a.tilePosition.y) - Mathf.Abs(b.tilePosition.y));
        int remainingDistance = Mathf.Abs(xDistance - yDistance);
        return remainingDistance;
    }

    private BaseTile GetTheLowestFCostNode(List<BaseTile> baseTileList)
    {
        BaseTile lowestFCostBaseTile = baseTileList[0];
        for (int i = 0; i < baseTileList.Count; i++)
        {
            if (baseTileList[i].fCost < lowestFCostBaseTile.fCost)
            {
                lowestFCostBaseTile = baseTileList[i];
            }
        }

        return lowestFCostBaseTile;
    }
}
