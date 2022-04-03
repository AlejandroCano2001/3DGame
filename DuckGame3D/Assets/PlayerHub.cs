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
    private ThirdPersonMovement playerMovement;

    void Start() 
    {
        playerStats = GetComponent<Stats>();
        playerMovement = GetComponent<ThirdPersonMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = playerStats.speed + playerMovement.addedSpeed;

        health.text = playerStats.health.ToString();
        Speed.text = speed.ToString();
        Shield.text = playerStats.shield.ToString();
        Damage.text = playerStats.damage.ToString();
    }
}
