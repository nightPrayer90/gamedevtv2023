using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Enemy))]
public class NewEnemyHealth : MonoBehaviour
{
    private Enemy enemy;

    [Header("Enemy Stats")]
    public int enemyHealth;
    private int enemyStartHealth; 

    [Header("Collision Control")]
    public List<ParticleCollisionEvent> collisionEvents;

    private bool isBurning = false;
    public bool canTakeDamage = true;
    private int burnTickCount = 0;
    public event EventHandler DieEvent;

    private bool[] canTakeLaserDamage = new bool[] { true, true, true, true, true };

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle methoden
    private void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
        enemyStartHealth = enemyHealth;
    }

    private void OnEnable()
    {
        // reset all Invokes
        CancelInvoke();

        collisionEvents = new List<ParticleCollisionEvent>();

        isBurning = false;
        canTakeDamage = true;

        burnTickCount = 0;
        enemyHealth = enemyStartHealth;

        canTakeLaserDamage[0] = true; //burning Damage
        canTakeLaserDamage[1] = true; // MW Laser 1
        canTakeLaserDamage[2] = true; // MW Laser 2
        canTakeLaserDamage[3] = true; // front Laser
        canTakeLaserDamage[4] = true; // Orbital Laser
    }
    #endregion


    /* **************************************************************************** */
    /* TAKE DAMAGE CONTROL--------------------------------------------------------- */
    /* **************************************************************************** */
    #region take damage control
    // Bullet Damage ---------------------------------------------------------
    private void OnParticleCollision(GameObject other)
    {
        if (canTakeDamage == true)
        {
            ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.
            var ps = other.GetComponent<ParticleBullet>();
            int damage = ps.bulletDamage;
            int damagetyp = ps.damageTyp;
            enemy.resultColor = enemy.hitColor;

            // damage from a bullet
            if (damagetyp == 0)
            {
                int ran = UnityEngine.Random.Range(0, 100);
                if (ran < enemy.playerWeaponController.shipData.bulletCritChance)
                {
                    damage = CalculateCritDamage(damage);
                    enemy.resultColor = enemy.critColor;
                    NovaOnDie(1);
                }

                // Take Damage
                AudioManager.Instance.PlaySFX("ImpactShot");

                // calculate Enemy Health
                enemyHealth -= damage;

                if (enemyHealth <= 0)
                {
                    // Trigger Explosion
                    ObjectPoolManager.SpawnObject(enemy.explosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem); // TODO - Explosion Object?

                    DieState();
                }
            }

            int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

            foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
            {
                Vector3 pos = collisionEvent.intersection;
                enemy.gameManager.DoFloatingText(pos, damage.ToString(), enemy.resultColor);
            }
        }
    }

    // Expolsion Damage ---------------------------------------------------------
    public void TakeExplosionDamage(int damage)
    {
        if (canTakeDamage == true)
        {
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // create object to die effect
                if (enemy._AOEreplacement != null)
                {
                    var replacement = Instantiate(enemy._AOEreplacement, transform.position, transform.rotation);

                    // make the die effect object explode
                    var rbs = replacement.GetComponentsInChildren<Rigidbody>();
                    foreach (var rb in rbs)
                    {
                        rb.AddExplosionForce(UnityEngine.Random.Range(-16, 128), transform.position, 1);  //collision.contacts[0].point;
                    }
                }
                DieState();
            }
        }
    }

    // Laser Damage ---------------------------------------------------------
    public void TakeLaserDamage(int damage, int index)
    {
        if (canTakeLaserDamage[index] == true)
        {
            AudioManager.Instance.PlaySFX("PlayerLaserHit");
            canTakeLaserDamage[index] = false;

            if (gameObject.activeSelf)
                StartCoroutine(InvokeCanGetLaserDamage(index));

            // calculate burning damage
            int ran = UnityEngine.Random.Range(0, 100);
            if (ran < enemy.playerWeaponController.shipData.burnDamageChance && isBurning == false)
            {
                isBurning = true;
                InvokeBurningDamage();
            }

            // Take Damage
            enemyHealth -= damage;

            if (enemyHealth <= 0)
            {
                // create object die effect
                if (enemy._burnReplacement != null)
                {
                    Instantiate(enemy._burnReplacement, transform.position, transform.rotation);
                }
                // die sound
                AudioManager.Instance.PlaySFX("PlayerLaserDie");

                DieState();
            }
        }
    }
    private IEnumerator InvokeCanGetLaserDamage(int index)
    {
        yield return new WaitForSeconds(0.3f);
        canTakeLaserDamage[index] = true;
    }

    // Calulate Damage Abilitys ---------------------------------------------------------
    public void InvokeBurningDamage()
    {
        if (enemy.burnParticleSystem != null)
            enemy.burnParticleSystem.Play();
        InvokeRepeating("TakeBurningDamage", .2f, 1f);
    }
    public void TakeBurningDamage()
    {
        int burningDamage = Mathf.CeilToInt(enemy.playerWeaponController.shipData.baseLaserTickDamage * (enemy.playerWeaponController.shipData.laserBurningTickDamangePercent) / 100);

        enemy.ShowDamageFromObjectsColor(burningDamage, enemy.burningColor);
        TakeLaserDamage(burningDamage, 0);

        burnTickCount++;

        if (burnTickCount > enemy.playerWeaponController.shipData.baseLaserTicks)
        {
            CancelInvoke("TakeBurningDamage");
            burnTickCount = 0;
            isBurning = false;
            if (enemy.burnParticleSystem != null)
                enemy.burnParticleSystem.Stop();
        }
    }

    public int CalculateCritDamage(int damage)
    {
        damage = Mathf.CeilToInt(damage * ((float)enemy.playerWeaponController.shipData.bulletCritDamage / 100));
        return damage;
    }
    #endregion



    /* **************************************************************************** */
    /* Die State ------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region Die State
    private void DieState()
    {
        canTakeDamage = false;

        if (enemy.burnParticleSystem != null) enemy.burnParticleSystem.Stop();
        if (enemy.engineParticle != null) enemy.engineParticle.Stop();

        if (DieEvent != null)
        {
            DieEvent.Invoke(this, new EventArgs());
            return;
        }

        // cancle all Invokes
        CancelInvoke();

        // calculate chance of explosion
        NovaOnDie(0);

        // drop an Item
        ItemDrop();

        // update player UI
        if (enemy.secondDimensionEnemy == false)
        {
            if (enemy.isGroundUnit == false) enemy.gameManager.UpdateEnemyCounter(-1);
            enemy.gameManager.UpdateEnemyToKill(1);
        }

        // instanstiate explosion
        if (enemy.explosionObject != null)
            ObjectPoolManager.SpawnObject(enemy.explosionObject, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);


        // pool (destroy) enemy object
        if (enemy.canPoolObject == true)
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        else
            Destroy(gameObject);
    }

    // nova on die Ability
    private void NovaOnDie(int novaTyp) //0=die 1=crit
    {
        if (enemy.novaOnDie != null && (enemy.upgradeChooseList.upgrades[52].upgradeIndexInstalled == 1 || enemy.upgradeChooseList.upgrades[23].upgradeIndexInstalled == 1))
        {
            Vector3 pos = new Vector3(0, 0, 0);
            float explosionRadius = 0;
            int novaDamage = 0;

            switch (novaTyp)
            {
                case 0: // Nova triggert from die
                    if (UnityEngine.Random.Range(0, 100) > 10)
                    {
                        return;
                    }
                    pos = transform.position;

                    explosionRadius = 1.5f * ( 1+ enemy.playerWeaponController.shipData.rocketAOERadius/100);
                    novaDamage = 10;

                    break;

                case 1: // Nova triggert from crit
                    if (UnityEngine.Random.Range(0, 100) > 5)
                    {
                        return;
                    }
                    pos = transform.position;

                    explosionRadius = 0.5f * (1+ enemy.playerWeaponController.shipData.rocketAOERadius/100);
                    novaDamage = 6;

                    break;
            }

            LayerMask layerMask = (1 << 6);
            if (enemy.gameManager.dimensionShift == true)
            {
                layerMask = (1 << 9);
            }

            // Audio
            AudioManager.Instance.PlaySFX("Playernova");


            // array of all Objects in the explosionRadius
            var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);

            foreach (var obj in surroundingObjects)
            {
                // get rigidbodys from all objects in range
                var rb = obj.GetComponent<Rigidbody>();
                if (rb == null) continue;

                // calculate distance between explosioncenter and objects in Range
                float distance = Vector3.Distance(pos, rb.transform.position);

                if (distance < explosionRadius)
                {
                    enemy.resultColor = enemy.hitColor;
                    float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
                    int adjustedDamage = Mathf.CeilToInt(novaDamage * scaleFactor);

                    if (enemy.upgradeChooseList.upgrades[54].upgradeIndexInstalled > 0)
                    {
                        int ran = UnityEngine.Random.Range(0, 100);
                        if (ran < enemy.playerWeaponController.shipData.bulletCritChance)
                        {
                            adjustedDamage = CalculateCritDamage(adjustedDamage);
                            enemy.resultColor = enemy.critColor;
                        }
                    }

                    // get EnemyHealthscript
                    EnemyHealth eHC = obj.GetComponent<EnemyHealth>();

                    if (eHC != null)
                    {
                        // show floating text
                        if (eHC.canTakeDamage == true)
                            enemy.gameManager.DoFloatingText(rb.transform.position, adjustedDamage.ToString(), enemy.resultColor);

                        // calculate enemy damage
                        eHC.TakeExplosionDamage(adjustedDamage);
                    }
                }
                rb.AddExplosionForce(400, pos, explosionRadius);
            }

            GameObject go = ObjectPoolManager.SpawnObject(enemy.novaOnDie, transform.position, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);
            go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;
        }
    }

    private void ItemDrop()
    {
        if (enemy.dropItem != null)
        {
            ObjectPoolManager.SpawnObject(enemy.dropItem, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);
        }
    }
    #endregion
}
