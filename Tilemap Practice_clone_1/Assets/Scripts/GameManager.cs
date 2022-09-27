using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : NetworkBehaviour
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

    public int gameManagerTick = 0;

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
    public void AddToPlayersThatHaveBeenReceived(Controller controller)
    {
        playersThatHaveBeenReceived.Add(controller);
        if (playersThatHaveBeenReceived.Count == playerList.Count)
        {
            playersThatHaveBeenReceived.Clear();
            tick.Invoke();
            //allPlayersReceived = true;
        }
    }

}