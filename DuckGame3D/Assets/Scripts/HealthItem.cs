using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Stats>().health = 100; 

            Destroy(gameObject);
        }
    }
}
