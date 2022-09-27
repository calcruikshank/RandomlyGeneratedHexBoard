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

    public Color col;


    Vector3 mousePosition;
    TileBase highlightTile;
    Tilemap highlightMap;// set these = to gamemanage.singleton.highlightmap TODO
    Tilemap baseMap;
    Tilemap environmentMap;
    Tilemap waterMap;
    protected Grid grid;
    Vector3Int previousCellPosition;

    Transform castle;
    Creature creatureSelected;
    Vector3Int currentLocalHoverCellPosition;
    Vector3Int cellPositionSentToClients;
    Vector3Int targetedCellPosition;

    [SerializeField] LayerMask creatureMask;

    Vector3Int placedCellPosition;

    public int mana = 1;
    public int drawTimeThreshold = 100;
    public int drawTimer;
    public int manaTimeThreshold = 100;
    public int manaTimer;
    int maxHandSize = 7;
    [SerializeField] List<CardInHand> cardsInDeck;
    List<CardInHand> cardsInHand = new List<CardInHand>();

    public GameObject cardSelected;
    public List<Vector3> allVertextPointsInTilesOwned = new List<Vector3>();

    [SerializeField] Transform cardParent;
    Transform instantiatedPlayerUI;

    Canvas canvasMain;

    public int tick = 0; //this is for determining basically everything
    public float tickTimer = 0f;
    float tickThreshold = .12f;
    public List<Vector3Int> clickQueueForTick = new List<Vector3Int>();
    List<Vector3Int> tempLocalPositionsToSend = new List<Vector3Int>();
    List<int> tempLocalIndecesOfCardsInHand = new List<int>();
    public List<int> IndecesOfCardsInHandQueue = new List<int>();

    public bool hasTickedSinceSendingLastMessage = true;
    bool locallySelectedCreature = false;
    public override void OnNetworkSpawn()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        GrabAllObjectsFromGameManager();
        SpawnHUDAndHideOnAllNonOwners();
        state = State.PlacingCastle;
        col = Color.red;

        col = new Color(
       UnityEngine.Random.Range(0f, 1f),
       UnityEngine.Random.Range(0f, 1f),
       UnityEngine.Random.Range(0f, 1f)
  );
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

        if (GameManager.singleton.playerList.Count < 3)
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
        if (tickTimer > tickThreshold && hasTickedSinceSendingLastMessage)
        {
            hasTickedSinceSendingLastMessage = false;
            tickTimer = 0f;
            SendAllInputsInQueue();
        }
        if (!hasTickedSinceSendingLastMessage && !GameManager.singleton.playerList.Contains(this)) //attempts to resend on a failed rpc
        {
            Debug.LogError("Failed sending message attempting to try again at tick " + tick);
            hasTickedSinceSendingLastMessage = false;
            tickTimer = 0f;
            SendAllInputsInQueue();
        }
        if (Input.GetMouseButtonDown(0))
        {
            cellPositionSentToClients = grid.WorldToCell(mousePosition);
            if (state == State.NothingSelected)
            {
                if (locallySelectedCreature)
                {
                    AddToTickQueueLocal(cellPositionSentToClients);
                    locallySelectedCreature = false;
                    return;
                }
                if (!CheckForRaycast())
                {
                    AddToTickQueueLocal(cellPositionSentToClients);
                }
            }
            else
            {
                AddToTickQueueLocal(cellPositionSentToClients);
            }
            return;
        }

    }


    #region regionOfTicks
    void AddToTickQueueLocal(Vector3Int positionSent)
    {
        locallySelectedCreature = false;
        tempLocalPositionsToSend.Add(positionSent);
    }
    void AddIndexOfCardInHandToTickQueueLocal(int index)
    {
        tempLocalIndecesOfCardsInHand.Add(index);
    }

    List<int> tempIndexOfCreatureOnBoard = new List<int>();
    void AddIndexOfCreatureOnBoard(int index)
    {
        tempIndexOfCreatureOnBoard.Add(index);
    }
    void AddToTickQueue(Vector3Int positionSent)
    {
        clickQueueForTick.Add(positionSent);
    }
    List<int> indecesOfCreaturesInQueue = new List<int>();
    void AddToCreatureOnBoardQueue(int index)
    {
        indecesOfCreaturesInQueue.Add(index);
    }
    void AddToIndexQueue(int indexSent)
    {
        IndecesOfCardsInHandQueue.Add(indexSent);
    }

    void SendAllInputsInQueue()
    {
        Message message = new Message();
        message.leftClicksWorldPos = tempLocalPositionsToSend;
        message.guidsForCards = tempLocalIndecesOfCardsInHand;
        message.guidsForCreatures = tempIndexOfCreatureOnBoard;
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
        for (int i = 0; i < tempIndexOfCreatureOnBoard.Count; i++)
        {
            indecesOfCreaturesInQueue.Add(tempIndexOfCreatureOnBoard[i]);
        }
        tempLocalPositionsToSend.Clear();
        tempLocalIndecesOfCardsInHand.Clear();
        tempIndexOfCreatureOnBoard.Clear();
        if (!GameManager.singleton.playersThatHaveBeenReceived.Contains(this))
        {
            GameManager.singleton.AddToPlayersThatHaveBeenReceived(this);
        }
    }

    void TranslateToFuntionalStruct(string jsonOfMessage)
    {
        Message receievedMessage = JsonUtility.FromJson<Message>(jsonOfMessage);

        if (receievedMessage.guidsForCards.Count > 0)
        {
            for (int i = 0; i < receievedMessage.guidsForCards.Count; i++)
            {
                AddToIndexQueue(receievedMessage.guidsForCards[i]);
            }
        }
        if (receievedMessage.guidsForCreatures.Count > 0)
        {
            for (int i = 0; i < receievedMessage.guidsForCreatures.Count; i++)
            {
                AddToCreatureOnBoardQueue(receievedMessage.guidsForCreatures[i]);
            }
        }
        if (receievedMessage.leftClicksWorldPos.Count > 0)
        {
            for (int i = 0; i < receievedMessage.leftClicksWorldPos.Count; i++)
            {
                AddToTickQueue(receievedMessage.leftClicksWorldPos[i]);
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
        manaTimer++;
        if (manaTimer > manaTimeThreshold)
        {
            mana++;
        }
        if (drawTimer > drawTimeThreshold)
        {
            DrawCard();
        }
        //order matters here bigtime later set this up in the enum
        for (int i = 0; i < IndecesOfCardsInHandQueue.Count; i++)
        {
            LocalSelectCardWithIndex(IndecesOfCardsInHandQueue[i]);
            Debug.Log(IndecesOfCardsInHandQueue[i] + " IndecesOfCardsInHandQueue at i + tick " + tick);
        }
        for (int i = 0; i < indecesOfCreaturesInQueue.Count; i++)
        {
            SetToCreatureOnFieldSelected(creaturesOwned[indecesOfCreaturesInQueue[i]]);
            Debug.Log(creaturesOwned[indecesOfCreaturesInQueue[i]] + " creaturesOwned at i + tick " + tick);
        }
        for (int i = 0; i < clickQueueForTick.Count; i++)
        {
            LocalLeftClick(clickQueueForTick[i]);
            Debug.Log(clickQueueForTick[i] + " local left click + " + tick);
        }
        clickQueueForTick.Clear();
        IndecesOfCardsInHandQueue.Clear();
        indecesOfCreaturesInQueue.Clear();
        hasTickedSinceSendingLastMessage = true;
    }
    #endregion

    void LocalLeftClick(Vector3Int positionSent)
    {
        switch (state)
        {
            case State.PlacingCastle:
                LocalPlaceCastle(positionSent);
                break;
            case State.NothingSelected:
                break;
            case State.CreatureInHandSelected:
                HandleCreatureInHandSelected(positionSent);
                break;
            case State.CreatureSelected:
                HandleCreatureOnBoardSelected(positionSent);
                break;
        }
    }

    void LocalPlaceCastle(Vector3Int positionSent)
    {
        placedCellPosition = positionSent;
        Vector3 positionToSpawn = highlightMap.GetCellCenterWorld(placedCellPosition);

        SetOwningTile(placedCellPosition);

        for (int i = 0; i < BaseMapTileState.singleton.GetBaseTileAtCellPosition(placedCellPosition).neighborTiles.Count; i++)
        {
            SetOwningTile(BaseMapTileState.singleton.GetBaseTileAtCellPosition(placedCellPosition).neighborTiles[i].tilePosition);
        }
        Transform instantiatedCaste = Instantiate(castle, positionToSpawn, Quaternion.identity);
        instantiatedCaste.GetComponent<MeshRenderer>().material.color = col;
        instantiatedCaste.GetComponent<Structure>().playerOwningStructure = this;
        SetStateToNothingSelected();
    }

    bool CheckForRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHitCardInHand, Mathf.Infinity))
        {
            if (raycastHitCardInHand.transform.GetComponent<CardInHand>() != null)
            {
                AddIndexOfCardInHandToTickQueueLocal(raycastHitCardInHand.transform.GetComponent<CardInHand>().indexOfCard);
                return true;
            }
        }
        if (Physics.Raycast(ray, out RaycastHit raycastHitCreatureOnBoard, Mathf.Infinity, creatureMask))
        {
            if (raycastHitCreatureOnBoard.transform.GetComponent<Creature>() != null)
            {
                if (raycastHitCreatureOnBoard.transform.GetComponent<Creature>().playerOwningCreature == this)
                {
                    locallySelectedCreature = true;
                    AddIndexOfCreatureOnBoard(raycastHitCreatureOnBoard.transform.GetComponent<Creature>().creatureID);
                    //SetToCreatureOnFieldSelected(raycastHitCreatureOnBoard.transform.GetComponent<Creature>());
                    return true;
                }
            }
        }
        return false;
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
                state = State.CreatureInHandSelected;
            }
        }

    }
    void HandleCreatureOnBoardSelected(Vector3Int positionSent)
    {
        targetedCellPosition = positionSent;
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

    Dictionary<int, Creature> creaturesOwned = new Dictionary<int, Creature>();
    void HandleCreatureInHandSelected(Vector3Int cellSent)
    {
        if (BaseMapTileState.singleton.GetBaseTileAtCellPosition(cellSent) == null)
        {
            //show error
            return;
        }
        if (BaseMapTileState.singleton.GetBaseTileAtCellPosition(cellSent).playerOwningTile == this)
        {
            if (BaseMapTileState.singleton.GetBaseTileAtCellPosition(cellSent).structureOnTile == null)
            {
                Vector3 positionToSpawn = BaseMapTileState.singleton.GetWorldPositionOfCell(cellSent);
                if (environmentMap.GetInstantiatedObject(cellSent))
                {
                    GameObject instantiatedObject = environmentMap.GetInstantiatedObject(cellSent);
                    if (instantiatedObject.GetComponent<ChangeTransparency>() == null)
                    {
                        instantiatedObject.AddComponent<ChangeTransparency>();
                    }
                    ChangeTransparency instantiatedObjectsChangeTransparency = instantiatedObject.GetComponent<ChangeTransparency>();
                    instantiatedObjectsChangeTransparency.ChangeTransparent(100);
                }
                GameObject instantiatedCreature = Instantiate(cardSelected, positionToSpawn, Quaternion.identity);
                instantiatedCreature.GetComponent<Creature>().SetToPlayerOwningCreature(this);
                creaturesOwned.Add(instantiatedCreature.GetComponent<Creature>().creatureID, instantiatedCreature.GetComponent<Creature>());
                SetStateToNothingSelected();
            }
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