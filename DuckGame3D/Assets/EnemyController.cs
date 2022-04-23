using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float radius = 10f;
    public float outOfRange = 25f;
    public Animator anim;
    public float attackingSpeed = 2f;

    Transform target;
    NavMeshAgent agent;

    // Attacking variables
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayers;

    public GameObject[] turrets;
    GameObject targetedTurret;

    public GameObject cure;

    private bool threatened = false;
    private bool playerFound = false;
    private bool hasBeenChasing = false;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = GetComponent<Stats>().speed;
        agent.autoRepath = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetComponent<Stats>().health);

        if(!playerFound)
        {
            if(targetedTurret == null)
            {
                targetedTurret = lookForNearestTurret();
            }
            else
            {
                //Debug.Log("Target: " + targetedTurret);

                agent.isStopped = false;
                agent.SetDestination(targetedTurret.transform.position);

                //Debug.Log("Distancia a torreta: " + Vector3.Distance(targetedTurret.transform.position, transform.position) + "Stopping distance: " + agent.stoppingDistance);

                if (Vector3.Distance(targetedTurret.transform.position, transform.position) <= agent.stoppingDistance + 0.2)
                {
                    //Debug.Log("Ya activado!");

                    targetedTurret.GetComponent<TurretMovement>().activateTurret();
                    targetedTurret = null;
                    agent.ResetPath();
                }
            }
        }

        if(gameObject.GetComponent<Stats>().health <= 50f)
        {
            if(cure != null)
            {
                agent.ResetPath();
                agent.SetDestination(cure.transform.position);
                agent.isStopped = false;

                if (Vector3.Distance(cure.transform.position, transform.position) <= agent.stoppingDistance + 0.2)
                {
                    Destroy(cure);
                    gameObject.GetComponent<Stats>().health = 100f;

                    Debug.Log("Zombie's health: " + gameObject.GetComponent<Stats>().health);

                    agent.ResetPath();
                }
            }
        }

        Bullet bullet = FindObjectOfType<Bullet>();

        //Debug.Log(bullet == null);

        if (bullet != null)
        {
            threatened = true;

            gameObject.transform.LookAt(bullet.transform);

            //Flee

            //agent.SetDestination(new Vector3(transform.position.x - 4, transform.position.y, transform.position.z));
        }
        else
        {
            threatened = false;
        }

        attackingSpeed -= Time.deltaTime;

        if(!GetComponent<Stats>().isDead)
        {
            if(target != null)
            {
                float distance = Vector3.Distance(target.position, transform.position);

                if (distance <= radius && !threatened && gameObject.GetComponent<Stats>().health > 50)
                {
                    playerFound = true;
                    agent.ResetPath();
                    agent.SetDestination(target.position);
                    hasBeenChasing = true;
                    agent.isStopped = false;

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
                else if(distance >= outOfRange)
                {
                    playerFound = false;

                    if(hasBeenChasing)
                    {
                        agent.ResetPath();
                        hasBeenChasing = false;
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

    private GameObject lookForNearestTurret()
    {
        GameObject nearestTurret = null;
        bool allActivated = true;

        foreach(GameObject turret in turrets)
        {
            if(!turret.GetComponent<TurretMovement>().getIsActivated())
            {
                allActivated = false;
            }
        }

        if(!allActivated)
        {
            nearestTurret = turrets[0];

            foreach (GameObject turret in turrets)
            {
                if ((Vector3.Distance(turret.transform.position, transform.position) <= Vector3.Distance(nearestTurret.transform.position, transform.position)
                    && !turret.GetComponent<TurretMovement>().getIsActivated()) || (!turret.GetComponent<TurretMovement>().getIsActivated() && nearestTurret.GetComponent<TurretMovement>().getIsActivated()))
                {
                    nearestTurret = turret;
                }
            }
        }

        //Debug.Log("Torreta seleccionada: " + nearestTurret.gameObject.name);

        return nearestTurret;
    }
}
