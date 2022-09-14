using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Controller : MonoBehaviour
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
    public TileBase highlightTile; 
    public Tilemap highlightMap;// set these = to gamemanage.singleton.highlightmap TODO
    public Tilemap baseMap;
    public Tilemap environmentMap;
    public Tilemap waterMap;
    public Grid grid;
    Vector3Int previousCellPosition;

    [SerializeField] Transform castle;
    Creature creatureSelected;
    Vector3Int currentCellPosition;
    [SerializeField] Transform testPrefabToSpawn;

    [SerializeField] LayerMask creatureMask;

    Vector3Int placedCellPosition;

    public int mana = 1 ;
    public float drawTimeThreshold = 5f;
    public float drawTimer;
    public float manaTimeThreshold = 5f;
    public float manaTimer;
    int maxHandSize = 7;
    [SerializeField]List<CardInHand> cardsInDeck;
    List<CardInHand> cardsInHand = new List<CardInHand>();

    public GameObject cardSelected;
    public List<Vector3> allVertextPointsInTilesOwned = new List<Vector3>();

    [SerializeField]Transform cardParent;
    // Start is called before the first frame update
    void Start()
    {
        state = State.PlacingCastle;
        mousePositionScript = GetComponent<MousePositionScript>();
        for (int i = 0; i < 3; i++)
        {
            DrawCard();
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        }
        return;
       
        if (Input.GetMouseButtonDown(0))
        {
            #region creatureSelected
            if (creatureSelected != null)
            {
                //determine if 
                Vector3 positionToTarget = GetWorldPositionOfCell();
                creatureSelected.SetMove(positionToTarget);
                creatureSelected = null;
                return;
            }
            //make sure to shoot a raycast to check to see if it hits a creature first
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, creatureMask))
            {
                if (raycastHit.transform.GetComponent<Creature>() != null)
                {
                    creatureSelected = raycastHit.transform.GetComponent<Creature>();
                    return;
                }
            }
            /*if (BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition) != null) 
            {
                creatureSelected = BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition);
                Debug.Log(creatureSelected);
                return;
            }*/
            #endregion

            #region spawningObjects
            Vector3 positionToSpawn = GetWorldPositionOfCell();
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
            Instantiate(testPrefabToSpawn, positionToSpawn, Quaternion.identity);
            #endregion
        }
    }

    void HandlePlacingCastle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            placedCellPosition = currentCellPosition;
            Vector3 positionToSpawn = highlightMap.GetCellCenterWorld(placedCellPosition);
            if (environmentMap.GetInstantiatedObject(placedCellPosition))
            {
                GameObject instantiatedObject = environmentMap.GetInstantiatedObject(placedCellPosition);
                Destroy(instantiatedObject);
            }
            SetOwningTile(placedCellPosition);
            if (Mathf.Abs(placedCellPosition.y % 2) ==1 )
            {
                SetOwningTile(new Vector3Int(placedCellPosition.x + 1, placedCellPosition.y + 1, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x + 1, placedCellPosition.y, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x + 1, placedCellPosition.y - 1, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x, placedCellPosition.y - 1, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x - 1, placedCellPosition.y, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x, placedCellPosition.y + 1, placedCellPosition.z));
            }
            if (Mathf.Abs(placedCellPosition.y % 2) == 0)
            {
                SetOwningTile(new Vector3Int(placedCellPosition.x, placedCellPosition.y + 1, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x + 1, placedCellPosition.y, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x - 1, placedCellPosition.y - 1, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x, placedCellPosition.y - 1, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x - 1, placedCellPosition.y, placedCellPosition.z));
                SetOwningTile(new Vector3Int(placedCellPosition.x - 1, placedCellPosition.y + 1, placedCellPosition.z));
            }

            Instantiate(castle, positionToSpawn, Quaternion.identity);
            state = State.NothingSelected;
        }
    }

    void HandleNothingSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity))
            {
                if (raycastHit.transform.GetComponent<CardInHand>() != null)
                {
                    Debug.Log("always go back to roiots cuzx ti works" + raycastHit.transform);
                    return;
                }
            }
        }
    }

    void HandleCreatureInHandSelected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            #region creatureSelected
            /*if (creatureSelected != null)
            {
                //determine if 
                Vector3 positionToTarget = GetWorldPositionOfCell();
                creatureSelected.SetMove(positionToTarget);
                creatureSelected = null;
                return;
            }*/
            //make sure to shoot a raycast to check to see if it hits a creature first
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, creatureMask))
            {
                if (raycastHit.transform.GetComponent<Creature>() != null)
                {
                    creatureSelected = raycastHit.transform.GetComponent<Creature>();
                    return;
                }
            }*/
            /*if (BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition) != null) 
            {
                creatureSelected = BaseMapTileState.singleton.GetCreatureAtTile(currentCellPosition);
                Debug.Log(creatureSelected);
                return;
            }*/
            #endregion

            #region spawningObjects
            Vector3 positionToSpawn = GetWorldPositionOfCell();
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

            //todo make a method for changing state to nothing selected
            state = State.NothingSelected;
            #endregion
        }
    }


    void HandleMana()
    {
        manaTimer += Time.deltaTime;
        if (manaTimer >= manaTimeThreshold)
        {
            mana++;
            Debug.Log(mana);
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
    Vector3 GetWorldPositionOfCell()
    {
        Vector3 worldPositionOfCell = highlightMap.GetCellCenterWorld(currentCellPosition);
        if (baseMap.GetInstantiatedObject(currentCellPosition))
        {
            worldPositionOfCell = new Vector3(worldPositionOfCell.x, worldPositionOfCell.y + baseMap.GetInstantiatedObject(currentCellPosition).gameObject.GetComponent<Collider>().bounds.size.y, worldPositionOfCell.z);
        }
        return worldPositionOfCell;
    }

    void SetOwningTile(Vector3Int cellPosition)
    {
        if (!tilesOwned.ContainsKey(cellPosition))
        {
            if (baseMap.GetInstantiatedObject(cellPosition) != null)
            {
                baseMap.GetInstantiatedObject(cellPosition).GetComponent<BaseTile>().SetOwnedByPlayer(this);
                tilesOwned.Add(cellPosition, baseMap.GetInstantiatedObject(cellPosition).GetComponent<BaseTile>());

            }
            else
            {
                waterMap.GetInstantiatedObject(cellPosition).GetComponent<BaseTile>().SetOwnedByPlayer(this);
                tilesOwned.Add(cellPosition, waterMap.GetInstantiatedObject(cellPosition).GetComponent<BaseTile>());
            }
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
        GameObject cardInHand = Instantiate(cardAddingToHand.gameObject, cardParent);
        cardsInHand.Add(cardAddingToHand);
    }




    public void SetToCardSelected(CardInHand cardInHand)
    {
        //todo check if card selected is a creature
        cardSelected = cardInHand.GameObjectToInstantiate.gameObject;
        state = State.CreatureInHandSelected;
    }

}
