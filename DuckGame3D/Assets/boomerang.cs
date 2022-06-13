using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomerang : MonoBehaviour
{
    public bool activated;
    public float rotationSpeed;

    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;

    public float damage;

    void Start()
    {
        activated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            //Debug.Log("Boomerang activated!");
            transform.localEulerAngles += Vector3.up * rotationSpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            DealDamage();
        }
        else
        {
            activated = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        Destroy(gameObject, 2f);
    }

    private void DealDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            //Debug.Log("Damage dealt: " + damage.ToString());

            enemy.gameObject.GetComponent<Stats>().TakeDamage(damage);
        }
    }
}
