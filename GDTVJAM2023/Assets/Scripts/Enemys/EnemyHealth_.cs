using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth_ : MonoBehaviour
{
    [Header("Objects")]


    [Header("Enemy Settings")]
    public float enemyHealth = 2.0f;
    [HideInInspector]public float enemyStartHealth;
    public int collisonDamage = 1;
    public float explosionForce = 5.0f;
    public bool expOrbSpawn = false;
    public bool secondDimensionEnemy = false;
    public bool canTakeDamage = true;
    public bool canPoolObject = true;

    public bool isBoss = false;
    public bool isMine = false;

    [Header("Enemy Weapons")]
    public List<EnemyParticleBullet> enemyWeapons;
    public List<ParticleSystem> enemyWeaponParticles;
    public int bulletDamage;
    public float fireRate;
    private bool isShooting = false;



    






    [Header("Laser burning Control")]
    public GameObject _burnReplacement;
    public ParticleSystem burnParticleSystem;
    
    private bool isBurning = false;
    private int burnTickCount = 0;
    public bool[] canTakeLaserDamage = new bool[5];



   
 
    /* **************************************************************************** */
    /* SHOOTING CONTROL------------------------------------------------------------ */
    /* **************************************************************************** */


    

    
}
