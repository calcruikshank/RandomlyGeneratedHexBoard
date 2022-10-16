using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTile : MonoBehaviour
{
    public Vector3Int tilePosition;
    Grid grid;
    Tilemap environmentMap;
    Tilemap baseTileMap;
    Tilemap waterTileMap;
    Creature creatureOnTile;
    GameObject environmentOnTile;
    public Structure structureOnTile;

    public Controller playerOwningTile;

    public List<BaseTile> neighborTiles;

    public Tilemap currentMapThatTileIsIn;

    public int gCost;
    public int hCost;
    public int fCost;
    public BaseTile cameFromBaseTile;
    public enum ManaType
    {
        Red,
        Green,
        Blue,
        Black,
        White
    }
    public ManaType manaType;
    public enum traversableType
    {
        Untraversable,
        OnlyFlying,
        SwimmingAndFlying,
        TraversableByAll
    }
    [SerializeField] public traversableType traverseType;
    private void Start()
    {
        SetupLR();
        grid = GameManager.singleton.grid;
        environmentMap = GameManager.singleton.enviornmentMap;
        baseTileMap = GameManager.singleton.baseMap;
        waterTileMap = GameManager.singleton.waterTileMap;
        tilePosition = grid.WorldToCell(this.transform.position);
        
        BaseMapTileState.singleton.AddToBaseTiles(tilePosition, this);
        
        environmentOnTile = environmentMap.GetInstantiatedObject(tilePosition);

        CalculateAllPoints();

        SetAllNeighborTiles();
    }

    void SetAllNeighborTiles()
    {
        if (Mathf.Abs(tilePosition.y % 2) == 1)
        {
            SetNeighborTile(new Vector3Int(tilePosition.x + 1, tilePosition.y + 1, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x + 1, tilePosition.y, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x + 1, tilePosition.y - 1, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x - 1, tilePosition.y, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x, tilePosition.y + 1, tilePosition.z));
        }
        if (Mathf.Abs(tilePosition.y % 2) == 0)
        {
            SetNeighborTile(new Vector3Int(tilePosition.x, tilePosition.y + 1, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x + 1, tilePosition.y, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x - 1, tilePosition.y - 1, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x - 1, tilePosition.y, tilePosition.z));
            SetNeighborTile(new Vector3Int(tilePosition.x - 1, tilePosition.y + 1, tilePosition.z));
        }
    }

    public void SetNeighborTile(Vector3Int cellPosiitonSent)
    {
        if (baseTileMap.GetInstantiatedObject(cellPosiitonSent) != null)
        {
            neighborTiles.Add(baseTileMap.GetInstantiatedObject(cellPosiitonSent).GetComponent<BaseTile>());
            return;
        }
        if (waterTileMap.GetInstantiatedObject(cellPosiitonSent) != null)
        {
            neighborTiles.Add(waterTileMap.GetInstantiatedObject(cellPosiitonSent).GetComponent<BaseTile>());
            return;
        }
    }

    internal Creature CreatureOnTile()
    {
        return creatureOnTile;
    }
    internal Structure StructureOnTile()
    {
        return structureOnTile;
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
    internal void AddStructureToTile(Structure structure)
    {
        structureOnTile = structure;
        if (environmentOnTile != null)
        {
            Destroy(environmentOnTile);
        }

    }
    internal void RemoveStructureFromTile(Structure structure)
    {
        structureOnTile = null;
    }

    public List<Vector3> worldPositionsOfVectorsOnGrid = new List<Vector3>();

    public Vector3 topRight;
    public Vector3 bottomRight;
    public Vector3 bottom;
    public Vector3 bottomLeft;
    public Vector3 topLeft;
    public Vector3 top;

    void CalculateAllPoints()
    {
        float y = 0;
        Vector3 worldPositionOfCell = new Vector3(this.transform.position.x, .21f, this.transform.position.z);
        topRight = new Vector3(grid.GetBoundsLocal(tilePosition).extents.x, y, grid.GetBoundsLocal(tilePosition).extents.z / 2) + worldPositionOfCell;
        worldPositionsOfVectorsOnGrid.Add(topRight);

        bottomRight = new Vector3(grid.GetBoundsLocal(tilePosition).extents.x, y, -grid.GetBoundsLocal(tilePosition).extents.z / 2) + worldPositionOfCell;
        worldPositionsOfVectorsOnGrid.Add(bottomRight);

        bottom = new Vector3(0, y, -grid.GetBoundsLocal(tilePosition).extents.z) + worldPositionOfCell;
        worldPositionsOfVectorsOnGrid.Add(bottom);
        bottomLeft = new Vector3(-grid.GetBoundsLocal(tilePosition).extents.x, y, -grid.GetBoundsLocal(tilePosition).extents.z / 2) + worldPositionOfCell;
        worldPositionsOfVectorsOnGrid.Add(bottomLeft);
        topLeft = new Vector3(-grid.GetBoundsLocal(tilePosition).extents.x, y, grid.GetBoundsLocal(tilePosition).extents.z / 2) + worldPositionOfCell;
        worldPositionsOfVectorsOnGrid.Add(topLeft);
        top = new Vector3(0, y, grid.GetBoundsLocal(tilePosition).extents.z) + worldPositionOfCell;
        worldPositionsOfVectorsOnGrid.Add(top);
        worldPositionsOfVectorsOnGrid.Add(topRight);
    }
    public void SetOwnedByPlayer(Controller playerOwningTileSent)
    {
        playerOwningTile = playerOwningTileSent;
        lr.startColor = (playerOwningTile.col);
        lr.endColor = (playerOwningTile.col);
        for (int i  = 0; i < worldPositionsOfVectorsOnGrid.Count; i++)
        {
            if (playerOwningTileSent.allVertextPointsInTilesOwned.Contains(worldPositionsOfVectorsOnGrid[i]))
            {
                playerOwningTileSent.allVertextPointsInTilesOwned.Remove(worldPositionsOfVectorsOnGrid[i]);
            }
            else
            {
                playerOwningTileSent.allVertextPointsInTilesOwned.Add(worldPositionsOfVectorsOnGrid[i]);
            }
        }
        lr.enabled = true;
        lr.positionCount = worldPositionsOfVectorsOnGrid.Count;
        lr.SetPositions(worldPositionsOfVectorsOnGrid.ToArray());
    }
    LineRenderer lr;
    GameObject lrGameObject;
    void SetupLR()
    {
        lrGameObject = new GameObject("LineRendererGameObject", typeof(LineRenderer));
        lrGameObject.transform.parent = this.transform;
        lr = lrGameObject.GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.alignment = LineAlignment.TransformZ;
        lr.transform.localEulerAngles = new Vector3(90, 0, 0);
        lr.sortingOrder = 1;
        lr.startWidth = .1f;
        lr.endWidth = .1f;
        lr.numCapVertices = 90;
        lr.numCornerVertices = 90;
        lr.material = GameManager.singleton.RenderInFrontMat;
    }
    internal void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

}
