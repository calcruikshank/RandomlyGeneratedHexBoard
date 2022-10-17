using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    public delegate void TickTookTooLong();
    public static event TickTookTooLong tickTookTooLong;
    public float timeBetweenLastTick;
    protected float timeBetweenTickCounter;
    public bool hasPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.singleton.tick += OnTick;
    }

    private void OnTick()
    {
        timeBetweenLastTick = timeBetweenTickCounter;
        timeBetweenTickCounter = 0;
        hasPaused = false;
        //Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {

        timeBetweenTickCounter += Time.deltaTime;
        if (timeBetweenTickCounter > 3f && hasPaused == false)
        {
            //Time.timeScale = 0;
            hasPaused = true;

            tickTookTooLong?.Invoke();
            Debug.Log(timeBetweenTickCounter);
        }
    }
}
