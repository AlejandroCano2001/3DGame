using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if(player)
        {
            if (player.GetComponent<Stats>().isDead)
            {
                gameOverCanvas.SetActive(true);
            }
        }
    }
}
