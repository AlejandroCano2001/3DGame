using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<Stats>().damage += 10; 

            Destroy(gameObject);
        }
    }
}
