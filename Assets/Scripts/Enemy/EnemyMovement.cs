using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    Transform player;
    PlayerHealth playerHealth;
    Health enemyHealth;
    UnityEngine.AI.NavMeshAgent nav;
    EnemyFOV sight;
    Animator anim;
    IAttack attack;

    EnemyPatrol enemyPatrol;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<Health>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        sight = GetComponentInChildren<EnemyFOV>();
        anim = GetComponent<Animator>();
        attack = GetComponent<IAttack>();

        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    void Update()
    {
        if (sight.playerInSight)
        {
            if (enemyPatrol != null && enemyPatrol.isInPatrol == true)
            {
                // we stop the patrol
                enemyPatrol.isInPatrol = false;
            }

            if (enemyHealth.GetCurrentHealth() > 0 && playerHealth.GetCurrentHealth() > 0)
            {
                // lookt at the player
                transform.LookAt(player);

                if (!attack.IsInAttackRange())
                {
                    anim.SetFloat("EnemyMove", 1.0f);
                    nav.enabled = true;
                    nav.SetDestination(player.position);
                }
                else
                {
                    nav.enabled = false;
                    anim.SetFloat("EnemyMove", 0.0f);
                }
            }            
        } else if (enemyPatrol != null && enemyPatrol.isInPatrol == true)
        {
            anim.SetFloat("EnemyMove", 1.0f);
        }
        else
        {
            anim.SetFloat("EnemyMove", 0.0f);
        }
    }

    public void ChangeNavSpeed(int amount)
    {
        nav.speed += amount;
    }
}