using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarManager : MonoBehaviour
{
    public Transform mortar;
    public Transform shootingSpot;
    public GameObject bulletPrefab;
    public float r = 25f;
    
    private Transform target;
    private float cadence = 4f;

    // Update is called once per frame
    void Update()
    {

        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        cadence -= Time.deltaTime;

        if (enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            target = enemy.transform;

            //Debug.Log("Distance: " + distance + "; Radius: " + r);

            if (distance <= r)
            {
                mortar.LookAt(target);

                if (cadence <= 0)
                {
                    Instantiate(bulletPrefab, shootingSpot.position, Quaternion.identity);
                    cadence = 8f;
                }
            }
        }
    }
}
