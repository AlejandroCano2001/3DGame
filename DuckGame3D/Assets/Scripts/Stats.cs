using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float health;
    public float speed;
    public float damage;
    public float shield;
    public Animator anim;
    
    public bool isDead = false;

    public void TakeDamage(float damage)
    {
        //Debug.Log("I've taken damage");
        anim.SetTrigger("TakeDamage");
        
        this.health = this.health - damage + (0.15f * this.shield);

        if(health <= 0 & !isDead)
        {
            Die();
        }
    }

    public void reduceSpeed()
    {
        float duration = 0;
        float initialSpeed = this.speed;

        this.speed = 2f;
    }

    public void Die()
    {
        //Animation clip
        anim.SetTrigger("Die");
        isDead = true;

        Invoke("destroyObject", 3f);

    }

    private void destroyObject()
    {
        Destroy(gameObject);
    }
}
