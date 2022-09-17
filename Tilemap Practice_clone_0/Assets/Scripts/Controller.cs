using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : NetworkBehaviour
{
    public State state;
    public enum State
    {
        NothingSelected,
        CreatureSelected,
        CreatureInHandSelected,
        PlacingCastle
    }


    //Could use a state machine if creature is selected change state to creature selected 
    //if card in hand is selected change state to placing card
    //if neither are selected change state to selecting
    //if environment is selected change state to environment selected
    //if environment card is selected change state to environment card selected

    Dictionary<Vector3Int, BaseTile> tilesOwned = new Dictionary<Vector3Int, BaseTile>();

    MousePositionScript mousePositionScript;

    Vector3 mousePosition;
    TileBase highlightTile;
    Tilemap highlightMap;// set these = to gamemanage.singleton.highlightmap TODO
    Tilemap baseMap;
    Tilemap environmentMap;
    Tilemap waterMap;
    Grid grid;
    Vector3Int previousCellPosition;

    Transform castle;
    Creature creatureSelected;
    Vector3Int currentCellPosition;
    Vector3Int targetedCellPosition;

    [SerializeField] LayerMask creatureMask;

    Vector3Int placedCellPosition;

    public int mana = 1;
    public float drawTimeThreshold = 5f;
    public float drawTimer;
    public float manaTimeThreshold = 5f;
    public float manaTimer;
    int maxHandSize = 7;
    [SerializeField] List<CardInHand> cardsInDeck;
    List<CardInHand> cardsInHand = new List<CardInHand>();

    public GameObject cardSelected;
    public List<Vector3> allVertextPointsInTilesOwned = new List<Vector3>();

    Transform cardParent;

    public override void OnNetworkSpawn()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        GrabAllObjectsFromGameManager();
        state = State.PlacingCastle;
        mousePositionScript = GetComponent<MousePositionScript>();
        for (int i = 0; i < 3; i++)
        {
            DrawCard();
        }
    }

    void GrabAllObjectsFromGameManager()
    {
        highlightTile = GameManager.singleton.highlightTile;
        highlightMap = GameManager.singleton.highlightMap;// set these = to gamemanage.singleton.highlightmap TODO
        baseMap = GameManager.singleton.baseMap;
        environmentMap = GameManager.singleton.enviornmentMap;
        waterMap = GameManager.singleton.waterTileMap;
        grid = GameManager.singleton.grid;
        castle = GameManager.singleton.castleTransform;
        cardParent = GameManager.singleton.cardParent;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        mousePosition = mousePositionScript.GetMousePositionWorldPoint();
        currentCellPosition = grid.WorldToCell(mousePosition);
        if (currentCellPosition != previousCellPosition)
        {
            highlightMap.SetTile(previousCellPosition, null);
            highlightMap.SetTile(currentCellPosition, highlightTile);
            previousCellPosition = currentCellPosition;
            //Debug.Log(baseMap.GetInstantiatedObject(currentCellPosition));
        }


        switch (state)
        {
            case State.PlacingCastle:
                HandlePlacingCastle();
                break;
            case State.NothingSelected:
                HandleNothingSelected();
                HandleMana();
                HandleDrawCards();
                break;
            case State.CreatureInHandSelected:
                HandleCreatureInHandSelected();
                HandleMana();
                HandleDrawCards();
                break;
            case State.CreatureSelected:
                HandleCreatureOnBoardSelected();
                HandleMana();
                HandleDrawCards();
                break;
        }
        return;
    }

    void HandlePlacingCastle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftClickHandlePlacingCastleServerRpc(currentCellPosition);
        }
    }

    void LocalPlaceCastle(Vector3Int positionSent)
    {
        placedCellPosition = positionSent;
        Vector3 positionToSpawn = highlightMap.GetCellCenterWorld(placedCellPosition);
        if (environmentMap.GetInstantiatedObject(placedCellPosition))
        {
            GameObject instantiatedObject = environmentMap.GetInstantiatedObject(placedCellPosition);
            Destroy(instantiatedObject);
        }

        SetOwningTile(placedCellPosition);

        for (int i = 0; i < BaseMapTileState.singleton.GetBaseTileAtCellPosition(placedCellPosition).neighborTiles.Count; i++)
        {
            SetOwningTile(BaseMapTileState.singleton.GetBaseTileAtCellPosition(placedCellPosition).neighborTiles[i].tilePosition);
        }
        Instantiate(castle, positionToSpawn, Quaternion.identity);
        Debug.Log(positionToSpawn);
        SetStateToNothingSelected();
    }

    void HandleNothingSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if (Physics.Raycast(ray, out RaycastHit raycastHitCardInHand, Mathf.Infinity))
            {
                if (raycastHitCardInHand.transform.GetComponent<CardInHand>() != null)
                {
                    SetToCardSelected(raycastHitCardInHand.transform.GetComponent<CardInHand>());
                    return;
                }
            }
            if (Physics.Raycast(ray, out RaycastHit raycastHitCreatureOnBoard, Mathf.Infinity, creatureMask))
            {
                if (raycastHitCreatureOnBoard.transform.GetComponent<Creature>() != null)
                {
                    SetToCreatureOnFieldSelected(raycastHitCreatureOnBoard.transform.GetComponent<Creature>());
                    return;
                }
            }
        }
    }

    void HandleCreatureOnBoardSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetedCellPosition = currentCellPosition;
            #region creatureSelected
            if (creatureSelected != null)
            {
                //determine if 
                //Vector3 positionToTarget = GetWorldPositionOfCell();

                //select a tile if the tile contains something that isnt targetable then find nearest unnocupied tile and move to it
                /*if (BaseMapTileState.singleton.GetCreatureAtTile(targetedCellPosition))
                {
                    Creature creatureOnTileHit = BaseMapTileState.singleton.GetCreatureAtTile(targetedCellPosition);
                    //if the creature is a friendly creature hit move to nearest tile
                    if (creatureOnTileHit.playerOwningCreature == this)
                    {
                        BaseTile tileToMoveTo = BaseMapTileState.singleton.GetNearestUnnocupiedBaseTileGivenCell(creatureSelected.tileCurrentlyOn, BaseMapTileState.singleton.GetBaseTileAtCellPosition(targetedCellPosition));
                        creatureSelected.SetMove(GetWorldPositionOfCell(tileToMoveTo.tilePosition));
                    }
                }*/
                creatureSelected.SetMove(BaseMapTileState.singleton.GetWorldPositionOfCell(targetedCellPosition));

                if (BaseMapTileState.singleton.GetBaseTileAtCellPosition(targetedCellPosition) == creatureSelected.tileCurrentlyOn) //this makes sure you can double click to stop the creature and also have it selected
                {
                    SetToCreatureOnFieldSelected(creatureSelected);
                    return;
                }
                creatureSelected = null;
                SetStateToNothingSelected();
            }
            #endregion
        }
    }

    void HandleCreatureInHandSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            #region spawningObjects
            Vector3 positionToSpawn = BaseMapTileState.singleton.GetWorldPositionOfCell(currentCellPosition);
            if (environmentMap.GetInstantiatedObject(currentCellPosition))
            {
                GameObject instantiatedObject = environmentMap.GetInstantiatedObject(currentCellPosition);
                if (instantiatedObject.GetComponent<ChangeTransparency>() == null)
                {
                    instantiatedObject.AddComponent<ChangeTransparency>();
                }
                ChangeTransparency instantiatedObjectsChangeTransparency = instantiatedObject.GetComponent<ChangeTransparency>();
                instantiatedObjectsChangeTransparency.ChangeTransparent(100);
            }
            Instantiate(cardSelected, positionToSpawn, Quaternion.identity);
            cardSelected.GetComponent<Creature>().playerOwningCreature = this;
            SetStateToNothingSelected();
            #endregion
        }
    }


    void HandleMana()
    {
        manaTimer += Time.deltaTime;
        if (manaTimer >= manaTimeThreshold)
        {
            mana++;
            manaTimer = 0f;
        }
    }

    void HandleDrawCards()
    {
        drawTimer += Time.deltaTime;
        if (drawTimer >= drawTimeThreshold)
        {
            DrawCard();
            drawTimer = 0f;
        }
    }

    void SetOwningTile(Vector3Int cellPosition)
    {
        tilesOwned.Add(cellPosition, BaseMapTileState.singleton.GetBaseTileAtCellPosition(cellPosition));
        BaseMapTileState.singleton.GetBaseTileAtCellPosition(cellPosition).SetOwnedByPlayer(this);
    }

    void DrawCard()
    {
        if (cardsInHand.Count >= maxHandSize)
        {
            return;
        }
        CardInHand cardAddingToHand = cardsInDeck[cardsInDeck.Count - 1];
        cardsInDeck.RemoveAt(cardsInDeck.Count - 1);
        GameObject cardInHand = Instantiate(cardAddingToHand.gameObject, cardParent);
        cardsInHand.Add(cardAddingToHand);
    }




    public void SetToCardSelected(CardInHand cardInHand)
    {
        //todo check if card selected is a creature
        cardSelected = cardInHand.GameObjectToInstantiate.gameObject;
        state = State.CreatureInHandSelected;
    }
    public void SetToCreatureOnFieldSelected(Creature creatureSelectedSent)
    {
        creatureSelected = creatureSelectedSent;
        state = State.CreatureSelected;
    }
    void SetStateToNothingSelected()
    {
        cardSelected = null;
        creatureSelected = null;
        state = State.NothingSelected;
    }





    #region RPCS

    [ServerRpc]
    private void LeftClickHandlePlacingCastleServerRpc(Vector3Int positionSent)
    {
        LeftClickHandlePlacingCastleClientRpc(positionSent);
    }

    [ClientRpc] private void LeftClickHandlePlacingCastleClientRpc(Vector3Int positionSent)
    {
        LocalPlaceCastle(positionSent);
    }
    #endregion
}
