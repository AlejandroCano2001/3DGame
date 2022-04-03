using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float radius = 10f;
    public Animator anim;
    public float attackingSpeed = 2f;

    Transform target;
    NavMeshAgent agent;

    // Attacking variables
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = GetComponent<Stats>().speed;
    }

    // Update is called once per frame
    void Update()
    {
        attackingSpeed -= Time.deltaTime;

        if(!GetComponent<Stats>().isDead)
        {
            if(target != null)
            {
                float distance = Vector3.Distance(target.position, transform.position);

                if (distance <= radius)
                {
                    agent.SetDestination(target.position);

                    if (distance <= agent.stoppingDistance)
                    {
                        //Attack
                        if(attackingSpeed <= 0)
                        {
                            Invoke("Attack", 0.5f);
                            anim.SetTrigger("Attack");
                            attackingSpeed = 2f;
                        }

                        //Face the target
                        FaceTarget();
                    }
                }

                anim.SetBool("Running", agent.velocity != Vector3.zero);
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void Attack()
    {
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
