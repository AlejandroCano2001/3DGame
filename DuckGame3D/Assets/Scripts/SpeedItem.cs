using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Stats>().speed += 2f; 

            Destroy(gameObject);
        }
    }
}
