using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrap : MonoBehaviour
{
    public float force;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Invoke("Launch", 1);
        }
    }

    private void Launch()
    {
        rb.AddForce(Vector3.up * force);
    }
}
