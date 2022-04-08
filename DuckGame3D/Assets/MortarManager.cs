using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarManager : MonoBehaviour
{
    public Transform mortar;
    
    private Transform target;
    private float rotationSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

        if(enemy != null)
        {
            target = enemy.transform;

            Debug.Log("Objeto encontrado: " + enemy.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mortar.LookAt(target);
    }
}
