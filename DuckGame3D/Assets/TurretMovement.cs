using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    public Transform gun;
    public Transform gunTip;    
    public float rotationSpeed = 10f;
    public float shootingSpeed = 1f;
    public GameObject bullet;

    private LineRenderer lineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 10f;

    private bool enemyFound = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        lineRenderer.SetPositions(initLaserPositions);
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        //lineRenderer.useWorldSpace = true;
    }

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
            //Debug.DrawRay(transform.position, gunTip.rotation * Vector3.forward * hit.distance, Color.blue);
            //Debug.Log("Did Hit " + hit.collider.gameObject.tag);

            if(hit.collider.gameObject.tag == "Player")
            {
                enemyFound = true;

                if(shootingSpeed >= 1 && !hit.collider.gameObject.GetComponent<Stats>().isDead)
                {
                    //Debug.Log("gunTip position: " + gun.transform.position);
                    //Debug.Log("hit point position: " + hit.point);

                    lineRenderer.SetPosition(0, gunTip.transform.position);
                    lineRenderer.SetPosition(1, hit.point);
                    lineRenderer.enabled = true;

                    hit.collider.gameObject.GetComponent<Stats>().TakeDamage(20f);

                    hit.collider.gameObject.GetComponent<Stats>().reduceSpeed();
                    shootingSpeed = 0;
                }
                else
                {
                    Invoke("TurnOffLaser", 0.1f);
                }
            }
            else
            {
                enemyFound = false;
            }
        }
    }

    private void TurnOffLaser()
    {
        lineRenderer.enabled = false;
    }
}
