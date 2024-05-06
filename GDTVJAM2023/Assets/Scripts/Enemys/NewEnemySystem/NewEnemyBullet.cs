using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class NewEnemyBullet : MonoBehaviour
{
    private Enemy enemy;

    public int bulletDamage;
    public float fireRate;
    public List<EnemyParticleBullet> enemyWeapons;
    public List<ParticleSystem> enemyWeaponParticles;

    private AudioSource shootSound;



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle methoden
    private void Awake()
    {
        shootSound = GetComponent<AudioSource>();
        enemy = gameObject.GetComponent<Enemy>();
    }

    private void OnDisable()
    {
        CancelInvoke("InvokeShooting");
    }
    #endregion



    /* **************************************************************************** */
    /* Bullet Control -------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Bullet Control

    public void StartShooting()
    {
        CancelInvoke("InvokeShooting");
        foreach (EnemyParticleBullet particle in enemyWeapons)
        {
            particle.bulletDamage = bulletDamage;
        }
        InvokeRepeating("InvokeShooting", 1f, fireRate);
    }

    public void InvokeShooting()
    {
        foreach (ParticleSystem particle in enemyWeaponParticles)
        {
            particle.Emit(1);
        }
        if (shootSound != null)
        {
            shootSound.volume = AudioManager.Instance.sfxVolume;
            shootSound.Play();
        }
    }

    public void StopShooting()
    {
        CancelInvoke("InvokeShooting");
    }

    #endregion
}


