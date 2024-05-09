using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int bulletDamage;
    public float fireRate;
    public List<ParticleSystem> enemyWeaponParticles;
    public List<EnemyParticleBullet> enemyWeapons = new();

    public bool isShooting = false;
    private AudioSource shootSound;
    private EnemyPlayerDetector playerDetector;
    private GameManager gameManager;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle methoden
    private void Awake()
    {
        shootSound = GetComponent<AudioSource>();
        playerDetector = GetComponent<EnemyPlayerDetector>();
        playerDetector.OnSwapState += HandleStateSwap;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameManager.OnDimensionSwap += HandleDimensionSwap;

        foreach (ParticleSystem item in enemyWeaponParticles)
        {
            EnemyParticleBullet ePB = item.gameObject.GetComponent<EnemyParticleBullet>();
            enemyWeapons.Add(ePB);
        }
    }

    private void HandleStateSwap(int state)
    {
        switch (state)
        {
            case 0:
                StopShooting();
                break;

            case 1:
                StartShooting();
                break;
        }
    }

    private void HandleDimensionSwap(bool isSecondDimension)
    {
        if (gameObject.activeSelf == true)
        {
            if (isSecondDimension == true)
            {
                StopShooting();
            }
            else
            {
                OnEnable();
            }
        }
    }

    private void OnEnable()
    {
        isShooting = false;
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
        if (isShooting == false)
        {
            CancelInvoke("InvokeShooting");
            foreach (EnemyParticleBullet particle in enemyWeapons)
            {
                particle.bulletDamage = bulletDamage;
            }
            InvokeRepeating("InvokeShooting", 1f, fireRate);
            isShooting = true;
        }
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
        if (isShooting == true)
        {
            CancelInvoke("InvokeShooting");
            isShooting = false;
        }
    }

    #endregion
}


