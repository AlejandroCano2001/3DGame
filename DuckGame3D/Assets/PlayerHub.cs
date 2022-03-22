using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHub : MonoBehaviour
{
    public Text health;
    public Text Speed;
    public Text Shield;
    public Text Damage;
    
    private Stats playerStats;

    void Start() 
    {
        playerStats = GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        health.text = playerStats.health.ToString();
        Speed.text = playerStats.speed.ToString();
        Shield.text = playerStats.shield.ToString();
        Damage.text = playerStats.damage.ToString();
    }
}
