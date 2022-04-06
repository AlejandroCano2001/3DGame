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
    private float rotVelocity = 10f;
    private bool isGrounded;
    private Rigidbody rb;

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
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                anim.SetTrigger("Jump");
            }

            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");

            Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;

            if (dir.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotVelocity * Time.deltaTime);

                Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

                rb.velocity = moveDir * speed * addedSpeed;
            }

            anim.SetFloat("Speed", rb.velocity.magnitude);

            if (Input.GetButtonDown("Fire1") && cadence >= 0.5f)
            {
                cadence = 0f;
                Attack();
            }
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
