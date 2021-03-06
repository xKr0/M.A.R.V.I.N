using UnityEngine;

public class EnemyHealth : Health
{
    public float sinkSpeed = 2.5f;
    public int scoreValue = 10;

    // the player shooting script (to add the exp)
    PlayerShooting playerShooting;

    // amount of xp given when we kill the monster
    public int xpGiven = 2;

    // animator to control animation
    Animator anim;

    CapsuleCollider capsuleCollider;
    bool isSinking;

    // for effect and take damage
    EnemyMovement enemyMovement;

    // to detect the player when the enemy get hit
    EnemyFOV sight;

    void Awake()
    {       
        anim = GetComponent<Animator>();
        hurtSound = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerShooting = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooting>();
        enemyMovement = GetComponentInParent<EnemyMovement>();
        currentEffect = new Effect(this, enemyMovement);
        sight = GetComponentInChildren<EnemyFOV>();
        InitHealth();
    }


    void Update()
    {
        if (!isDead)
        {
            // calculate the effect to apply to the entity
            currentEffect.applyEffects();
        }
    }

    public void TakeDamage(int damage)
    {       
        // we applied the damage to the entity
        Damaged(damage);
    }

    public void TakeDamage(BulletScript bullet)
    {
        if (isDead)
            return;

        int damage = currentEffect.getHurt(bullet);

        // we applied the damage to the entity
        Damaged(damage);

        // if the enemy didn't saw the player yet, but he got hit 
        if (!sight.disabled)
        {
            sight.PlayerDetected(true);
        }
    }

    // Override the Health function to define the hurt behavior
    public override void HurtAnim()
    {        
        anim.SetTrigger("EnemyHurt");
    }

    // Override the Death Function to define the death behavior
    public override void Death()
    {
        anim.SetBool("EnemyDead", true);
        anim.SetBool("PlayerInRange", false);

        // Turn the capsule collider into a trigger so shots can pass through it.
        capsuleCollider.isTrigger = true;

        // we give the amount of xp to the player max energy
        playerShooting.energyMax += xpGiven;
        // we also give this energy to the current amount of energy of the player
        playerShooting.currentEnergy += xpGiven;

        GetComponent<AudioSource>().clip = deathClip;
        GetComponent<AudioSource>().Play();

        // Find and disable the Nav Mesh Agent.
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
        //GetComponent<Rigidbody>().isKinematic = true;

        // Increase the score by the enemy's score value.
        //ScoreManager.score += scoreValue;

        // After 2 second destory the enemy.
        Destroy(gameObject, 2f);
    }
}
