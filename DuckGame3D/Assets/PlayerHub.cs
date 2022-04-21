using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHub : MonoBehaviour
{
    public Text health;
    public Text Speed;
    public Text Shield;
    public Text Damage;
    public Text time;
    
    private Stats playerStats;
    private ThirdPersonMovement playerMovement;
    private TimeSpan stopwatch;
    private TimeSpan aux;

    void Start() 
    {
        stopwatch = new TimeSpan(0, 5, 0, 0);
        playerStats = GetComponent<Stats>();
        playerMovement = GetComponent<ThirdPersonMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(stopwatch.CompareTo(new TimeSpan(0, 0, 0, 0)) > 0)
        {
            aux = stopwatch.Subtract(new TimeSpan(0, 0, 0, 1));
            stopwatch = aux;
        }

        float speed = playerStats.speed + playerMovement.addedSpeed;

        health.text = playerStats.health.ToString();
        Speed.text = speed.ToString();
        Shield.text = playerStats.shield.ToString();
        Damage.text = playerStats.damage.ToString();
        time.text = stopwatch.ToString();
    }

    public TimeSpan getStopWatch()
    {
        return this.stopwatch;
    }
}
