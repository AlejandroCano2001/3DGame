using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject winningCanvas;
    public GameObject player;
    public PlayerHub hub;
    public Transform zombieSpawner;
    public GameObject zombie;

    private float timer = 30f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        gameOverCanvas.SetActive(false);
        winningCanvas.SetActive(false);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            Instantiate(zombie, zombieSpawner.position, Quaternion.identity);
            timer = 30f;
        }

        if(player)
        {
            if (player.GetComponent<Stats>().isDead)
            {
                gameOverCanvas.SetActive(true);
                //Invoke("EndGame", 5f);
            }

            if (!player.GetComponent<Stats>().isDead && hub.getStopWatch().CompareTo(new TimeSpan(0, 0, 0, 0)) == 0)
            {
                winningCanvas.SetActive(true);
                Invoke("EndGame", 5f);
            }
        }
    }

    private void EndGame()
    {
        Time.timeScale = 0f;
    }
}
