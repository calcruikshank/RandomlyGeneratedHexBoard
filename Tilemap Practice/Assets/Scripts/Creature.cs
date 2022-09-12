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
    float speed = 1f;
    float UsageRate; // the rate at which the minion can use abilities/ attack 

    Tilemap baseTileMap;
    Vector3Int currentCellPosition;

    BaseTile tileCurrentlyOn;
    BaseTile previousTilePosition;

    Vector3 targetPosition;

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
        creatureState = CreatureState.Moving;
    }

    public void Move()
    {
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
