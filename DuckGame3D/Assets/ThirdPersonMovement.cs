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
    public GameObject boomerang;

    private float speed;
    private float rotVelocity = 8f;
    private bool isGrounded;
    private Rigidbody rb;
    private bool justJumped = false;
    private float throwPower = 40f;

    // Attacking variables
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;

    public GameObject[] turrets;

    public GameObject deactivateKey;

    // Lista de animaciones de combate

    private void Start() {
        
        speed = GetComponent<Stats>().speed;
        rb = GetComponent<Rigidbody>();
        deactivateKey.SetActive(false);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        cadence += Time.deltaTime;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!GetComponent<Stats>().isDead)
        {
            if (justJumped && isGrounded)
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            throwBoomerang();
        }

        if (Vector3.Distance(transform.position, turrets[0].transform.position) <= 2)
        {
            deactivateKey.SetActive(true);
        }
        else if (Vector3.Distance(transform.position, turrets[1].transform.position) <= 2)
        {
            deactivateKey.SetActive(true);
        }
        else if (Vector3.Distance(transform.position, turrets[2].transform.position) <= 2)
        {
            deactivateKey.SetActive(true);
        }
        else if (Vector3.Distance(transform.position, turrets[3].transform.position) <= 2)
        {
            deactivateKey.SetActive(true);
        }
        else
        {
            deactivateKey.SetActive(false);
        }

        if (Vector3.Distance(transform.position, turrets[0].transform.position) <= 2 && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Turn it off!");
            turrets[0].GetComponent<TurretMovement>().deactivateTurret();
        }
        else if (Vector3.Distance(transform.position, turrets[1].transform.position) <= 2 && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Turn it off!");
            turrets[1].GetComponent<TurretMovement>().deactivateTurret();
        }
        else if (Vector3.Distance(transform.position, turrets[2].transform.position) <= 2 && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Turn it off!");
            turrets[2].GetComponent<TurretMovement>().deactivateTurret();
        }
        else if (Vector3.Distance(transform.position, turrets[3].transform.position) <= 2 && Input.GetKey(KeyCode.E))
        {
            Debug.Log("Turn it off!");
            turrets[3].GetComponent<TurretMovement>().deactivateTurret();
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

    public void throwBoomerang()
    {
        GameObject boomerangInstance = Instantiate(boomerang, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.5f, gameObject.transform.position.z), Quaternion.identity);
        boomerangInstance.GetComponent<Rigidbody>().AddForce(transform.forward * throwPower, ForceMode.Impulse);
        boomerangInstance.GetComponent<Rigidbody>().AddTorque(boomerangInstance.transform.TransformDirection(Vector3.right) * 100, ForceMode.Impulse);
    }
}
