using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    public Transform gun;
    public Transform gunTip;    
    public float rotationSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        gun.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        RaycastHit hit;
        if(Physics.Raycast(gunTip.position, gunTip.rotation * Vector3.forward, out hit))
        {
            Debug.DrawRay(transform.position, gunTip.rotation * Vector3.forward * hit.distance, Color.blue);
            Debug.Log("Did Hit " + hit.collider.gameObject.tag);

            if(hit.collider.gameObject.tag == "Player")
            {
                hit.collider.gameObject.GetComponent<Stats>().TakeDamage(1f);
            }
        }
    }
}
