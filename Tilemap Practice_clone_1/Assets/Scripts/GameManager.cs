using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    public State state;
    public Grid grid;
    public Tilemap baseMap;
    public Tilemap enviornmentMap;
    public Tilemap waterTileMap;
    public Tilemap highlightMap;
    public Material RenderInFrontMat;
    public Material TransparentSharedMat;
    public Material OpaqueSharedMat;
    public Transform castleTransform;
    public TileBase highlightTile;

    public Transform cardParent;
    public List<Controller> playerList = new List<Controller>();
    public List<Controller> playersThatHaveBeenReceived = new List<Controller>();

    public delegate void Tick();
    public event Tick tick;

    float tickTimer = 0f;
    float tickThreshold = .24f;

    int playerCount; //TODO set this equal to players in scene and return if a player has not hit

    public int creatureGuidCounter;
    private void Awake()
    {
        if (singleton != null) Destroy(this);
        singleton = this;
        state = State.Setup;
    }
    public enum State
    {
        Setup, //The state for placing your castle
        Game, 
        End //Setup for scaling
    }

    private void Update()
    {
        if (playerList.Count < 3) return;
        tickTimer += Time.deltaTime;
        if (tickTimer > tickThreshold)
        {
            if (allPlayersReceived)
            {
                tickTimer = 0;

                tick.Invoke();
                playersThatHaveBeenReceived.Clear();
                allPlayersReceived = false;
            }
            else
            {
                Debug.Log(tickTimer);
                //Debug.Break();
            }
        }
    }

    bool allPlayersReceived = false;
    public void AddToPlayersThatHaveBeenReceived(Controller controller)
    {
        playersThatHaveBeenReceived.Add(controller);
        if (playersThatHaveBeenReceived.Count == playerList.Count)
        {
            allPlayersReceived = true ;
        }
    }


}
