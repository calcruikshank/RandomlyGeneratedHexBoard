using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Creature : MonoBehaviour
{
    public CreatureState creatureState;
    public enum CreatureState
    {
        Summoned, //On The turn created
        Attack,
        UseAbility,
        Moving
        //not sure if i need a tapped state yet trying to keep it as simple as possible
    }

    LineRenderer lr;
    GameObject lrGameObject;
    int range = 1; //num of tiles that can attack
    float speed = 1f; //move speed
    float UsageRate; // the rate at which the minion can use abilities/ attack 

    Tilemap baseTileMap;
    Vector3Int currentCellPosition;

    BaseTile tileCurrentlyOn;
    BaseTile previousTilePosition;

    Vector3 targetPosition;

    Vector3[] positions = new Vector3[2];
    Grid grid;
    private void Awake()
    {
        creatureState = CreatureState.Summoned;
    }

    private void Start()
    {
        grid = GameManager.singleton.grid;
        baseTileMap = GameManager.singleton.baseMap;
        currentCellPosition = grid.WorldToCell(this.transform.position);
        tileCurrentlyOn = BaseMapTileState.singleton.GetBaseTileAtCellPosition(currentCellPosition);
        previousTilePosition = tileCurrentlyOn;
        tileCurrentlyOn.AddCreatureToTile(this);
        SetupLR();
        
    }
    void SetupLR()
    {
        lrGameObject = new GameObject("LineRendererGameObject", typeof(LineRenderer));
        lr = lrGameObject.GetComponent<LineRenderer>();
        lr.enabled = false;
        lr.alignment = LineAlignment.TransformZ;
        lr.transform.localEulerAngles = new Vector3(90, 0, 0);
        lr.sortingOrder = 1000;
        lr.startWidth = .2f;
        lr.endWidth = .2f;
        lr.numCapVertices = 1;
        lr.material = GameManager.singleton.RenderInFrontMat;
    }

    void Update()
    {
        switch (creatureState)
        {
            case CreatureState.Moving:
                Move();
                break;
        }
    }
    public void SetMove(Vector3 positionToTarget)
    {
        targetPosition = positionToTarget;
        positions[0] = this.transform.position;
        positions[1] = targetPosition;
        lr.enabled = true;
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);
        creatureState = CreatureState.Moving;
    }

    public void Move()
    {
        positions[0] = this.transform.position;
        lr.SetPositions(positions);
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, speed * Time.deltaTime);

        
        currentCellPosition = grid.WorldToCell(this.transform.position);
        tileCurrentlyOn = BaseMapTileState.singleton.GetBaseTileAtCellPosition(currentCellPosition); 
        if (previousTilePosition != tileCurrentlyOn)
        {
            Debug.Log(tileCurrentlyOn);
            previousTilePosition.RemoveCreatureFromTile(this);
            previousTilePosition = tileCurrentlyOn;
        }
        tileCurrentlyOn.AddCreatureToTile(this);
    }
}
