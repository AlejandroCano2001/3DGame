using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    public Transform gun;
    public Transform gunTip;    
    public float rotationSpeed = 10f;
    public float shootingSpeed = 1f;

    private bool enemyFound = false;

    // Update is called once per frame
    void Update()
    {
        shootingSpeed += Time.deltaTime;

        if(!enemyFound)
        {
            gun.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        RaycastHit hit;
        if(Physics.Raycast(gunTip.position, gunTip.rotation * Vector3.forward, out hit))
        {
            Debug.DrawRay(transform.position, gunTip.rotation * Vector3.forward * hit.distance, Color.blue);
            Debug.Log("Did Hit " + hit.collider.gameObject.tag);

            if(hit.collider.gameObject.tag == "Player")
            {
                enemyFound = true;

                if(shootingSpeed >= 1 && !hit.collider.gameObject.GetComponent<Stats>().isDead)
                {
                    // ESTO SERÁ TEMPORAL, YA QUE MÁS ADELANTE HARÉ QUE DISPARE UNA "BALA" LA CUAL SE ENCARGARÁ DE HACER DAÑO AL JUGADOR
                    hit.collider.gameObject.GetComponent<Stats>().TakeDamage(20f);
                    hit.collider.gameObject.GetComponent<Stats>().reduceSpeed();
                    shootingSpeed = 0;
                }
            }
        }
        else
        {
            enemyFound = false;
        }
    }
}
