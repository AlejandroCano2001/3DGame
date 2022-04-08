using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public Transform cam;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 10;
    public Animator anim;
    public float cadence = 1f;
    public float addedSpeed = 1f;

    private float speed;
    private float rotVelocity = 8f;
    private bool isGrounded;
    private Rigidbody rb;
    private bool justJumped = false;

    // Attacking variables
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;

    // Lista de animaciones de combate

    private void Start() {
        
        speed = GetComponent<Stats>().speed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        cadence += Time.deltaTime;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!GetComponent<Stats>().isDead)
        {
            if (justJumped)
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                anim.SetTrigger("Jump");
                justJumped = false;
            }

            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

            Quaternion rotation = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotVelocity * Time.deltaTime);

            rb.MovePosition(transform.position + rotation * dir * speed * Time.deltaTime);

            anim.SetFloat("Speed", dir.magnitude);
        }
    }

    private void Update()
    {
        speed = GetComponent<Stats>().speed;

        if (Input.GetButtonDown("Jump"))
        {
            justJumped = true;
        }

        if (Input.GetButtonDown("Fire1") && cadence >= 0.5f)
        {
            cadence = 0f;
            Attack();
        }
    }

    public void Attack()
    {
        // anim.SetTrigger(AnimacionAleatoria);
        anim.SetTrigger("Punch");

        Invoke("DealDamage", 0.25f);
    }

    private void DealDamage()
    {
        float damage = GetComponent<Stats>().damage;

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            //Debug.Log("Damage dealt: " + damage.ToString());

            enemy.gameObject.GetComponent<Stats>().TakeDamage(damage);
        }
    }
}
