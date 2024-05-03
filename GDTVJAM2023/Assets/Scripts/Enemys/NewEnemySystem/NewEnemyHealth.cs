using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemyHealth : Enemy
{
    public float enemyHealth = 2.0f;
    [HideInInspector] public float enemyStartHealth;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle methoden
    private void Awake()
    {
        enemyStartHealth = enemyHealth;
    }

    private void OnEnable()
    {
     /*   collisionEvents = new List<ParticleCollisionEvent>();

        enemyHealth = enemyStartHealth;
        isBurning = false;

        if (canPoolObject == true)
            canTakeDamage = true;

        burnTickCount = 0;
        CancelInvoke();
        if (burnParticleSystem != null)
            burnParticleSystem.Stop();

        if (engineParticle != null)
            engineParticle.Play();

        isdied = false;
        canTakeLaserDamage[0] = true; //burning Damage
        canTakeLaserDamage[1] = true; // MW Laser 1
        canTakeLaserDamage[2] = true; // MW Laser 2
        canTakeLaserDamage[3] = true; // front Laser
        canTakeLaserDamage[4] = true; // Orbital Laser*/
    }
    #endregion

}
