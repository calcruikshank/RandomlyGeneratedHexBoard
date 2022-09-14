using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTile : MonoBehaviour
{
    Vector3Int tilePosition;
    Grid grid;
    Tilemap environmentMap;
    Tilemap baseTileMap;
    Creature creatureOnTile;
    GameObject environmentOnTile;

    public Controller playerOwningTile;

    private void Start()
    {
        SetupLR();
        grid = GameManager.singleton.grid;
        environmentMap = GameManager.singleton.enviornmentMap;
        baseTileMap = GameManager.singleton.baseMap;
        tilePosition = grid.WorldToCell(this.transform.position);
        BaseMapTileState.singleton.AddToBaseTiles(tilePosition, this);
        
        environmentOnTile = environmentMap.GetInstantiatedObject(tilePosition);
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

    List<Vector3> worldPositionsOfVectorsOnGrid = new List<Vector3>();
    public void SetOwnedByPlayer(Controller playerOwningTileSent)
    {
        float y = 0;
        Vector3 worldPositionOfCell = new Vector3( this.transform.position.x, .21f, this.transform.position.z) ;
        playerOwningTile = playerOwningTileSent;
        Vector3 topRight = new Vector3(grid.GetBoundsLocal(tilePosition).extents.x, y, grid.GetBoundsLocal(tilePosition).extents.z / 2);
        worldPositionsOfVectorsOnGrid.Add(topRight + worldPositionOfCell);
        Vector3 bottomRight = new Vector3(grid.GetBoundsLocal(tilePosition).extents.x, y, -grid.GetBoundsLocal(tilePosition).extents.z / 2);
        worldPositionsOfVectorsOnGrid.Add(bottomRight + worldPositionOfCell);
        Vector3 bottom = new Vector3(0, y, -grid.GetBoundsLocal(tilePosition).extents.z);
        worldPositionsOfVectorsOnGrid.Add(bottom + worldPositionOfCell);
        Vector3 bottomLeft = new Vector3(-grid.GetBoundsLocal(tilePosition).extents.x, y, -grid.GetBoundsLocal(tilePosition).extents.z / 2);
        worldPositionsOfVectorsOnGrid.Add(bottomLeft + worldPositionOfCell);
        Vector3 topLeft = new Vector3(-grid.GetBoundsLocal(tilePosition).extents.x, y, grid.GetBoundsLocal(tilePosition).extents.z / 2);
        worldPositionsOfVectorsOnGrid.Add(topLeft + worldPositionOfCell);
        Vector3 top = new Vector3(0, y, grid.GetBoundsLocal(tilePosition).extents.z);
        worldPositionsOfVectorsOnGrid.Add(top + worldPositionOfCell);
        worldPositionsOfVectorsOnGrid.Add(topRight + worldPositionOfCell);

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
}
