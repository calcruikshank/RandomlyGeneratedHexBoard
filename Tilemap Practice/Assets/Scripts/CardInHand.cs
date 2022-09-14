using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInHand : MonoBehaviour
{
    [SerializeField]public Transform GameObjectToInstantiate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCard()
    {
        //get player ui associated 
        //for now ill just say gamemanager.player at one TODO
        //todo check if player has enough mana to select card
        GameManager.singleton.playerList[0].SetToCardSelected(this);
    }
}
