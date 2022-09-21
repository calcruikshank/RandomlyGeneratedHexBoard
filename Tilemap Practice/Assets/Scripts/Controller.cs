using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    Vector3Int currentLocalHoverCellPosition;
    Vector3Int cellPositionSentToClients;
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

    [SerializeField] Transform cardParent;
    Transform instantiatedPlayerUI;

    Canvas canvasMain;

    public int tick = 0; //this is for determining basically everything
    public float tickTimer; //this is for determining basically everything
    float tickThreshold = .12f; //this is for determining basically everything
    public List<Vector3> clickQueueForTick = new List<Vector3>();
    List<Vector3> tempLocalPositionsToSend = new List<Vector3>();
    List<int> tempLocalIndecesOfCardsInHand = new List<int>();
    List<int> IndecesOfCardsInHandQueue = new List<int>();
    public override void OnNetworkSpawn()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        GrabAllObjectsFromGameManager();
        SpawnHUDAndHideOnAllNonOwners();
        state = State.PlacingCastle;
        mousePositionScript = GetComponent<MousePositionScript>();
        for (int i = 0; i < 3; i++)
        {
            DrawCard();
        }
    }

    void GrabAllObjectsFromGameManager()
    {
        GameManager.singleton.tick += OnTick;
        canvasMain = FindObjectOfType<Canvas>();
        highlightTile = GameManager.singleton.highlightTile;
        highlightMap = GameManager.singleton.highlightMap;// set these = to gamemanage.singleton.highlightmap TODO
        baseMap = GameManager.singleton.baseMap;
        environmentMap = GameManager.singleton.enviornmentMap;
        waterMap = GameManager.singleton.waterTileMap;
        grid = GameManager.singleton.grid;
        castle = GameManager.singleton.castleTransform;
        GameManager.singleton.playerList.Add(this);
    }
    void SpawnHUDAndHideOnAllNonOwners()
    {
        instantiatedPlayerUI = Instantiate(cardParent, canvasMain.transform);
        if (!IsOwner)
        {
            instantiatedPlayerUI.GetComponent<Image>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*switch (state)
        {
            case State.PlacingCastle:
                break;
            case State.NothingSelected:
                HandleMana();
                HandleDrawCards();
                break;
            case State.CreatureInHandSelected:
                HandleMana();
                HandleDrawCards();
                break;
            case State.CreatureSelected:
                HandleMana();
                HandleDrawCards();
                break;
        }*/
        if (!IsOwner)
        {
            return;
        }




        currentLocalHoverCellPosition = grid.WorldToCell(mousePosition);
        mousePosition = mousePositionScript.GetMousePositionWorldPoint();
        if (currentLocalHoverCellPosition != previousCellPosition)
        {
            highlightMap.SetTile(previousCellPosition, null);
            highlightMap.SetTile(currentLocalHoverCellPosition, highlightTile);
            previousCellPosition = currentLocalHoverCellPosition;
            //Debug.Log(baseMap.GetInstantiatedObject(currentCellPosition));
        }

        tickTimer += Time.deltaTime;
        if (tickTimer > tickThreshold)
        {
            tickTimer = 0;
            SendAllInputsInQueue();
            //sendEmptyEvent
        }
        if (Input.GetMouseButtonDown(0))
        {

            if (state == State.NothingSelected)
            {
                HandleNothingSelected();
            }
            else
            {
                AddToTickQueueLocal(mousePosition);
            }
            return;
        }

    }

    void AddToTickQueueLocal(Vector3 positionSent)
    {
        tempLocalPositionsToSend.Add(positionSent);
    }
    void AddIndexOfCardInHandToTickQueueLocal(int index)
    {
        tempLocalIndecesOfCardsInHand.Add(index);
    }
    void AddToTickQueue(Vector3 positionSent)
    {
        clickQueueForTick.Add(positionSent);
    }
    void AddToIndexQueue(int indexSent)
    {
        IndecesOfCardsInHandQueue.Add(indexSent);
    }

    void SendAllInputsInQueue()
    {
        if (!GameManager.singleton.playersThatHaveBeenReceived.Contains(this))
        {
            GameManager.singleton.AddToPlayersThatHaveBeenReceived(this);
        }
        Message message = new Message();
        message.leftClicksWorldPos = tempLocalPositionsToSend;
        message.guidsForCards = tempLocalIndecesOfCardsInHand;
        //set guids of struct
        string messageString = JsonUtility.ToJson(message);
        SendMessageServerRpc(messageString);

        for (int i = 0; i < tempLocalPositionsToSend.Count; i++)
        {
            clickQueueForTick.Add(tempLocalPositionsToSend[i]);
        }
        for (int i = 0; i < tempLocalIndecesOfCardsInHand.Count; i++)
        {
            IndecesOfCardsInHandQueue.Add(tempLocalIndecesOfCardsInHand[i]);
        }
        tempLocalPositionsToSend.Clear();
        tempLocalIndecesOfCardsInHand.Clear();
    }

    void TranslateToFuntionalStruct(string jsonOfMessage)
    {
        Message receievedMessage = JsonUtility.FromJson<Message>(jsonOfMessage);
        if (receievedMessage.leftClicksWorldPos.Count > 0)
        {
            for (int i = 0; i < receievedMessage.leftClicksWorldPos.Count; i++)
            {
                AddToTickQueue(receievedMessage.leftClicksWorldPos[i]);
            }
        }
        if (receievedMessage.guidsForCards.Count > 0)
        {
            for (int i = 0; i < receievedMessage.guidsForCards.Count; i++)
            {
                AddToIndexQueue(receievedMessage.guidsForCards[i]);
            }
        }
        if (!GameManager.singleton.playersThatHaveBeenReceived.Contains(this))
        {
            GameManager.singleton.AddToPlayersThatHaveBeenReceived(this);
        }
    }
    void OnTick()
    {
        tick++;
        for (int i = 0; i < clickQueueForTick.Count; i++)
        {
            LocalLeftClick(clickQueueForTick[i]);
        }
        for (int i = 0; i < IndecesOfCardsInHandQueue.Count; i++)
        {
            LocalSelectCardWithIndex(IndecesOfCardsInHandQueue[i]);
        }
        clickQueueForTick.Clear();
        IndecesOfCardsInHandQueue.Clear();
        GameManager.singleton.playersThatHaveBeenReceived.Clear();
        if (tempLocalPositionsToSend.Count >= 1 || tempLocalIndecesOfCardsInHand.Count > 0) //or creatures on board selected .count > 0
        {
            if (!GameManager.singleton.playersThatHaveBeenReceived.Contains(this))
            {
                GameManager.singleton.AddToPlayersThatHaveBeenReceived(this);
            }
        }
    }

    void LocalLeftClick(Vector3 positionSent)
    {
        cellPositionSentToClients = grid.WorldToCell(positionSent);
        switch (state)
        {
            case State.PlacingCastle:
                HandlePlacingCastle();
                break;
            case State.NothingSelected:
                break;
            case State.CreatureInHandSelected:
                HandleCreatureInHandSelected();
                break;
            case State.CreatureSelected:
                HandleCreatureOnBoardSelected();
                break;
        }
    }
    void HandlePlacingCastle()
    {
        LocalPlaceCastle(cellPositionSentToClients);
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
        SetStateToNothingSelected();
    }

    void HandleNothingSelected()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHitCardInHand, Mathf.Infinity))
        {
            if (raycastHitCardInHand.transform.GetComponent<CardInHand>() != null)
            {
                AddIndexOfCardInHandToTickQueueLocal(raycastHitCardInHand.transform.GetComponent<CardInHand>().indexOfCard);
                //do an rpc and send an index 
                //SetToCardSelected(raycastHitCardInHand.transform.GetComponent<CardInHand>());
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

    void LocalSelectCardWithIndex(int indexOfCardSelected)
    {
        CardInHand cardToSelect;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            if (cardsInHand[i].indexOfCard == indexOfCardSelected)
            {
                cardToSelect = cardsInHand[i];
                cardSelected = cardToSelect.GameObjectToInstantiate.gameObject;
                Debug.Log(indexOfCardInHandSelected + " Index of card selected " + cardToSelect.gameObject.name);
                state = State.CreatureInHandSelected;
            }
        }

    }
    void HandleCreatureOnBoardSelected()
    {
        targetedCellPosition = cellPositionSentToClients;
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

    void HandleCreatureInHandSelected()
    {
        Vector3 positionToSpawn = BaseMapTileState.singleton.GetWorldPositionOfCell(cellPositionSentToClients);
        if (environmentMap.GetInstantiatedObject(cellPositionSentToClients))
        {
            GameObject instantiatedObject = environmentMap.GetInstantiatedObject(cellPositionSentToClients);
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
        if (!tilesOwned.ContainsKey(cellPosition))
        {
            tilesOwned.Add(cellPosition, BaseMapTileState.singleton.GetBaseTileAtCellPosition(cellPosition));
            BaseMapTileState.singleton.GetBaseTileAtCellPosition(cellPosition).SetOwnedByPlayer(this);
        }
    }

    void DrawCard()
    {
        if (cardsInHand.Count >= maxHandSize)
        {
            return;
        }
        CardInHand cardAddingToHand = cardsInDeck[cardsInDeck.Count - 1];
        cardsInDeck.RemoveAt(cardsInDeck.Count - 1);

        GameObject cardInHand = Instantiate(cardAddingToHand.gameObject, instantiatedPlayerUI);
        cardInHand.GetComponent<CardInHand>().indexOfCard = cardsInHand.Count;
        cardsInHand.Add(cardInHand.GetComponent<CardInHand>());
    }



    int indexOfCardInHandSelected;
    /*public void SetToCardSelected(CardInHand cardInHand)
    {
        //todo check if card selected is a creature
        cardSelected = cardInHand.GameObjectToInstantiate.gameObject;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            if (cardsInHand[i].indexOfCard == cardInHand.indexOfCard)
            {
                indexOfCardInHandSelected = i;
            }
        }
        Debug.Log(indexOfCardInHandSelected + " Index of card selected " + cardInHand.gameObject.name);
        state = State.CreatureInHandSelected;
    }*/
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
    private void SendMessageServerRpc(string json)
    {
        SendMessageClientRpc(json);
    }
    [ClientRpc]
    private void SendMessageClientRpc(string json)
    {
        if (IsOwner) return;
        TranslateToFuntionalStruct(json);
    }
    #endregion
}
