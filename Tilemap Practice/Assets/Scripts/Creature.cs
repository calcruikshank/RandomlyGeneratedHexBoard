using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Creature : MonoBehaviour
{
    public State creatureState;
    float speed;
    float UsageRate; // the rate at which the minion can use abilities/ attack 

    Tilemap baseTileMap;
    Vector3Int currentCellPosition;

    BaseTile tileCurrentlyOn;

    Grid grid;
    private void Awake()
    {
        creatureState = State.Summoned;
    }

    private void Start()
    {
        grid = GameManager.singleton.grid;
        baseTileMap = GameManager.singleton.baseMap;
        currentCellPosition = grid.WorldToCell(this.transform.position);
        tileCurrentlyOn = BaseMapTileState.singleton.GetBaseTileAtCellPosition(currentCellPosition);
        tileCurrentlyOn.AddCreatureToTile(this);
    }
    public enum State
    {
        Summoned, //On The turn created
        Attack,
        UseAbility,
        Moving
        //not sure if i need a tapped state yet trying to keep it as simple as possible
    }
}
