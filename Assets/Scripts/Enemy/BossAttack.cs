﻿using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 1f;
    public int attackDamage = 40;
    public int AOEattackDamage = 60;

    public float attackRange = 15f;

    Animator anim;
    GameObject player;
    PlayerHealth playerHealth;
    BossHealth bossHealth;
    bool playerInRange;
    protected float timer;

    // bullet shoot by boss
    public GameObject bossProj;

    // point where the bullet come from
    public GameObject staff;

    // speed of the bullet
    public int bossBulletSpeed = 280;

    // counter for pattern attacks
    int patternCount;
    // Si boss faible PV changement de pattern
    bool venerePattern;

    // L'effet qui sera lancé pour l'AOE
    BossFight scriptZoneBoss;


    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        bossHealth = GetComponent<BossHealth>();
        anim = GetComponent<Animator>();
        patternCount = 0;
        venerePattern = false;
        // The zone depends on the number of weapon
        int nbWeapon = player.GetComponent<PlayerShooting>().GetMaxWeapon();
        // Recupere la zone du boss pour pouvoir lancer l'effet de l'AOE
        switch (nbWeapon)
        {
            case 2:
                scriptZoneBoss = GameObject.FindGameObjectWithTag("IceGameplay").GetComponent<BossFight>();
                break;
            case 3:
                scriptZoneBoss = GameObject.FindGameObjectWithTag("FireGameplay").GetComponent<BossFight>();
                break;
            case 4:
                scriptZoneBoss = GameObject.FindGameObjectWithTag("ForestGameplay").GetComponent<BossFight>();
                break;
            case 5:
                scriptZoneBoss = GameObject.FindGameObjectWithTag("FinalGameplay").GetComponent<BossFight>();
                break;
        }
        
    }


    protected void Update()
    {
        timer += Time.deltaTime;

        if(bossHealth.GetCurrentHealth() <= 400)
        {
            venerePattern = true;
        }

        if (IsReadyToAttack())
        {
            scriptZoneBoss.StopAOE();
            anim.SetBool("PlayerInRange", true);
            PatternAttack();
        }
        else
        {
            anim.SetBool("PlayerInRange", false);
        }

        if (playerHealth.IsDead())
        {
            // so it doesn't keep attacking it
            anim.SetBool("PlayerInRange", false);
            anim.SetTrigger("PlayerDead");
        }
    }


    protected void PatternAttack()
    {
        
        if (!venerePattern)
        {
            if (patternCount % 6 < 5)
            {
                Attack();
            }
            else
            {
                AOEAttack();
            }      
        }
        else
        {
            if (patternCount % 3 < 2)
            {
                Attack();
            }
            else
            {
                AOEAttack();
            }
        }
        patternCount += 1;
    }


    void AOEAttack()
    {
        timer = 0f;

        //anim.SetInteger("NumAttack", 1);
        anim.SetTrigger("Numun");

        // On lance l'effet
        scriptZoneBoss.LaunchAOE();

        playerHealth.TakeDamage(AOEattackDamage);
    }

    // override parent method Attack()
    void Attack()
    {
        timer = 0f;

        //anim.SetInteger("NumAttack", 0);
        anim.SetTrigger("Numdeux");
    }

   bool IsReadyToAttack()
    {
        return (timer >= timeBetweenAttacks && !bossHealth.IsDead() && IsInAttackRange());
    }

    bool IsInAttackRange()
    {

        double distToPlayer = Mathf.Sqrt(Mathf.Pow((this.transform.position.x - player.transform.position.x), 2)
            + Mathf.Pow((this.transform.position.z - player.transform.position.z), 2));

        //Debug.Log(distToPlayer);
        return (distToPlayer <= attackRange);
    }

    

    public void LaunchBossBullet()
    {
        // we launch the bullet
        GameObject bullet = (GameObject)Instantiate(bossProj, staff.transform.position - new Vector3(0f,1.5f,0f), Quaternion.identity);
        bullet.gameObject.name = "Bullet";
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bossBulletSpeed);
    }
}
