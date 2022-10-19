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
    public Material rangeIndicatorMat;
    public Material OpaqueSharedMat;
    public Transform castleTransform;
    public TileBase highlightTile;

    public Transform cardParent;
    public List<Controller> playerList = new List<Controller>();
    public List<Controller> playersThatHaveBeenReceived = new List<Controller>();

    public delegate void Tick();
    public event Tick tick;
    public event Tick tickTookTooLong;

    public int gameManagerTick = 0;
    //public float tickTimeAverage;
    int playerCount; //TODO set this equal to players in scene and return if a player has not hit

    public int creatureGuidCounter;

    public int endingX;
    public int endingY;
    public int startingX;
    public int startingY;

    private void Awake()
    {
        if (singleton != null) Destroy(this);
        singleton = this;
        state = State.Setup;
    }
    private void Update()
    {
        if (playerList.Count < 2)
        {
            return;
        }
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
            gameManagerTick++;
            //tickTimeAverage = totalTickTime / gameManagerTick;
            //allPlayersReceived = true;
        }
    }
    public List<CardInHand> Shuffle(List<CardInHand> alpha)
    {
        for (int i = 0; i < alpha.Count; i++)
        {
            CardInHand temp = alpha[i];
            int randomIndex = UnityEngine.Random.Range(i, alpha.Count);
            alpha[i] = alpha[randomIndex];
            alpha[randomIndex] = temp;
        }
        return alpha;
    }

}
