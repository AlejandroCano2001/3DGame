using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    public float ShotSpeed = 10f;
    public float g = -9.81f;

    private Transform pointA;
    private Transform pointB;

    private Vector3 speed;
    private Vector3 Gravity;
    private Vector3 currentAngle;

    private float time;
    private float dTime = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        pointA = transform;
        pointB = GameObject.FindGameObjectWithTag("Enemy").transform;

        time = Vector3.Distance(pointA.position, pointB.position) / ShotSpeed;
        speed = new Vector3((pointB.position.x - pointA.position.x) / time,
                (pointB.position.y - pointA.position.y) / time - 0.5f * g * time, (pointB.position.z - pointA.position.z) / time);

        Gravity = Vector3.zero;

        Destroy(gameObject, time);
    }

    void FixedUpdate()
    {

        // v=gt

        Gravity.y = g * (dTime += Time.fixedDeltaTime);


        // Desplazamiento analógico

        transform.position += (speed + Gravity) * Time.fixedDeltaTime;


        // Rotación en radianes: Mathf.Rad2Deg

        currentAngle.x = -Mathf.Atan((speed.y + Gravity.y) / speed.z) * Mathf.Rad2Deg;


        // Establecer el ángulo actual

        transform.eulerAngles = currentAngle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Stats>().TakeDamage(40f);
            Destroy(gameObject, 0.25f);
        }
    }

    public Transform getZoneAttack()
    {
        return this.pointB;
    }
}
