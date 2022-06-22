using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject player;
    public float speed = 5f;
    public float radius = 1.5f;
    public Animator animator;

    private bool enemyFound = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 250;
    }

    // Update is called once per frame
    void Update()
    {
        followplayer(speed);


        RaycastHit hit;
        if (Physics.Raycast(rb.position, rb.rotation * Vector3.forward, out hit))
        {
            //Debug.DrawRay(transform.position, gunTip.rotation * Vector3.forward * hit.distance, Color.blue);
            //Debug.Log("Did Hit " + hit.collider.gameObject.tag);

            if (hit.collider.gameObject.tag == "Enemy")
            {
                enemyFound = true;

                Debug.Log("Enemy found!");

                Attack();
            }
            else
            {
                enemyFound = false;
            }
        }
    }

    private void followplayer(float speed)
    {
        rb.transform.LookAt(player.transform.position);

        Vector3 direction = player.transform.position - transform.position;
        float factor = direction.magnitude * speed;

        if (Vector3.Distance(player.transform.position, transform.position) > radius)
        {
            //Debug.Log("Distancia con respecto al coche: " + Vector3.Distance(player.transform.position, transform.position));
            rb.AddRelativeForce(new Vector3(player.transform.position.x, player.transform.position.y, Mathf.Abs(player.transform.position.z)) * factor);

            animator.SetFloat("Speed", rb.velocity.magnitude);
        }
        else if(rb.velocity != Vector3.zero)
        {
            rb.velocity = rb.velocity * 0.85f;
        }
        else
        {
            rb.AddForce(Vector3.zero);
        }
    }

    private void Attack()
    {
        //TODO
    }
}
