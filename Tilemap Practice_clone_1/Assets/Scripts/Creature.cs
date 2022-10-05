using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Creature : MonoBehaviour
{
    public CreatureState creatureState;

    [SerializeField] Transform colorIndicator;


    public List<BaseTile> allTilesWithinRange;
    public int creatureID;
    public enum CreatureState
    {
        Summoned, //On The turn created
        Attack,
        UseAbility,
        Moving,
        Idle
        //not sure if i need a tapped state yet trying to keep it as simple as possible
    }

    public Controller playerOwningCreature;

    LineRenderer lr;
    GameObject lrGameObject;
    [SerializeField] int range; //num of tiles that can attack
    float speed = 1f; //move speed
    float UsageRate; // the rate at which the minion can use abilities/ attack 

    Tilemap baseTileMap;
    public Vector3Int currentCellPosition;

    public BaseTile tileCurrentlyOn;
    public BaseTile previousTilePosition;

    Vector3 targetPosition;
    Vector3 actualPosition;

    Vector3[] positions = new Vector3[2];

    float timeBetweenLastTickOnMove;

    protected Grid grid;
    private void Awake()
    {
        creatureState = CreatureState.Summoned;
    }

    private void Start()
    {
        GameManager.singleton.tick += OnTick;
        grid = GameManager.singleton.grid;
        baseTileMap = GameManager.singleton.baseMap;
        currentCellPosition = grid.WorldToCell(this.transform.position);
        tileCurrentlyOn = BaseMapTileState.singleton.GetBaseTileAtCellPosition(currentCellPosition);
        previousTilePosition = tileCurrentlyOn;
        tileCurrentlyOn.AddCreatureToTile(this);
        SetupLR();
        actualPosition = this.transform.position;
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


    protected virtual void Update()
    {
        switch (creatureState)
        {
            case CreatureState.Moving:
                //Move();
                VisualMove();
                break;
        }
    }
    void OnTick()
    {
        switch (creatureState)
        {
            case CreatureState.Moving:
                Move();
                break;
        }
    }
    public virtual void SetMove(Vector3 positionToTarget, float timeBetweenLastTickOnMoveSent)
    {
        timeBetweenLastTickOnMove = timeBetweenLastTickOnMoveSent;
           Vector3Int targetedCellPosition = grid.WorldToCell(new Vector3(positionToTarget.x, 0, positionToTarget.z));
        int numOfTilesFromTarget = BaseMapTileState.singleton.GetNumberOfTilesBetweenTwoTiles(tileCurrentlyOn, BaseMapTileState.singleton.GetBaseTileAtCellPosition(targetedCellPosition));
        SetNewTargetPosition(positionToTarget);
        creatureState = CreatureState.Moving;
    }

    protected void SetNewTargetPosition(Vector3 positionToTarget)
    {
        targetPosition = positionToTarget;
        positions[0] = this.transform.position;
        positions[1] = targetPosition;
        lr.enabled = true;
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);
    }

    public void Move()
    {
        Vector3Int targetedCellPosition = grid.WorldToCell(new Vector3(targetPosition.x, 0, targetPosition.z));
        actualPosition = Vector3.MoveTowards(actualPosition, new Vector3(targetPosition.x, actualPosition.y, targetPosition.z), speed * timeBetweenLastTickOnMove);
        Debug.Log(timeBetweenLastTickOnMove + " TIme between last tick on move");
        currentCellPosition = grid.WorldToCell(new Vector3(actualPosition.x, 0, actualPosition.z));
        if (BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition) == null)
        {
            tileCurrentlyOn = BaseMapTileState.singleton.GetBaseTileAtCellPosition(currentCellPosition);
        }
        if (previousTilePosition != tileCurrentlyOn)
        {
            CalculateAllTilesWithinRange();
            Debug.Log(currentCellPosition);
            previousTilePosition.RemoveCreatureFromTile(this);
            previousTilePosition = tileCurrentlyOn;
            int numOfTilesFromTarget = BaseMapTileState.singleton.GetNumberOfTilesBetweenTwoTiles(tileCurrentlyOn, BaseMapTileState.singleton.GetBaseTileAtCellPosition(targetedCellPosition));
        }
        tileCurrentlyOn.AddCreatureToTile(this);

        if (tileCurrentlyOn.neighborTiles.Contains(BaseMapTileState.singleton.GetBaseTileAtCellPosition(targetedCellPosition))) //if your next to the tile you targeted and 
        {
            if (BaseMapTileState.singleton.GetCreatureAtTile(targetedCellPosition) != null)
            {
                //SetNewTargetPosition(BaseMapTileState.singleton.GetWorldPositionOfCell(currentCellPosition));
                if (BaseMapTileState.singleton.GetCreatureAtTile(targetedCellPosition).playerOwningCreature == this.playerOwningCreature && BaseMapTileState.singleton.GetCreatureAtTile(targetedCellPosition) != this)
                {
                    /*BaseTile newBaseTileToMoveTo = BaseMapTileState.singleton.GetNearestBaseTileGivenCell(tileCurrentlyOn, BaseMapTileState.singleton.GetBaseTileAtCellPosition(targetedCellPosition));
                    SetNewTargetPosition(BaseMapTileState.singleton.GetWorldPositionOfCell(newBaseTileToMoveTo.tilePosition));*/
                }
            }
        }
        if ((new Vector3(actualPosition.x, targetPosition.y, actualPosition.z) - targetPosition).magnitude < .02f)
        {
            SetStateToIdle();
        }
    }

    protected void VisualMove()
    {
        float valueToAdd = 0f;
        positions[0] = this.transform.position;
        lr.SetPositions(positions);
        float distanceFromActualPosition = (this.transform.position - actualPosition).magnitude;
        this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(targetPosition.x, this.transform.position.y, targetPosition.z), speed * Time.deltaTime);
    }

    internal void SetToPlayerOwningCreature(Controller controller)
    {
        this.playerOwningCreature = controller;
        colorIndicator.GetComponent<SpriteRenderer>().color = controller.col;
        creatureID = GameManager.singleton.creatureGuidCounter;
        GameManager.singleton.creatureGuidCounter++;
    }

    void SetStateToIdle()
    {
        tileCurrentlyOn.RemoveCreatureFromTile(this);
        lr.enabled = false;
        Debug.Log("setting state to idle and tick " + playerOwningCreature.tick);
        this.transform.position = targetPosition;
        actualPosition = targetPosition;
        currentCellPosition = grid.WorldToCell(new Vector3(this.transform.position.x, 0, this.transform.position.z));
        tileCurrentlyOn = BaseMapTileState.singleton.GetBaseTileAtCellPosition(currentCellPosition);
        tileCurrentlyOn.AddCreatureToTile(this);
        creatureState = CreatureState.Idle;
    }


    void CalculateAllTilesWithinRange()
    {
        allTilesWithinRange.Clear();
        int xthreshold;
        int threshold;
        for (int x = 0; x < range + 1; x++)
        {
            for (int y = 0; y < range + 1; y++)
            {
                xthreshold = range - x;
                threshold = range + xthreshold;
                if (y + x > threshold && currentCellPosition.y % 2 == 0)
                {
                    continue;
                }
                allTilesWithinRange.Add(BaseMapTileState.singleton.GetBaseTileAtCellPosition(new Vector3Int(currentCellPosition.x + x, currentCellPosition.y + y, currentCellPosition.z)));
                allTilesWithinRange.Add(BaseMapTileState.singleton.GetBaseTileAtCellPosition(new Vector3Int(currentCellPosition.x - x, currentCellPosition.y + y, currentCellPosition.z)));
                allTilesWithinRange.Add(BaseMapTileState.singleton.GetBaseTileAtCellPosition(new Vector3Int(currentCellPosition.x + x, currentCellPosition.y - y, currentCellPosition.z)));
                allTilesWithinRange.Add(BaseMapTileState.singleton.GetBaseTileAtCellPosition(new Vector3Int(currentCellPosition.x - x, currentCellPosition.y - y, currentCellPosition.z)));
            }
        }
        for (int i = 0; i < allTilesWithinRange.Count; i++) 
        {
            allTilesWithinRange[i].SetOwnedByPlayer(this.playerOwningCreature);
        }
    }
}
