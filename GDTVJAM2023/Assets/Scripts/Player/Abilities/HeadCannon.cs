using System.Collections.Generic;
using UnityEngine;

public class HeadCannon : MonoBehaviour
{
    [Header("Head Cannon")]
    public int bulletDamage = 2;
    public int fireSalveMax;
    public float reloadSalveInterval = 2f;
    public float reloadShotInterval = 0.1f;
    public float rotationSpeed = 5f; 
    public float detectionRange = 10f;


    [Header("Game Objects")]
    public List<ParticleSystem> bulletParticleSystem;
    public List<ParticleBullet> particleBullet;
    private GameObject nearestEnemy = null;
    private GameManager gameManager;
    private GameObject player;
    private UpgradeChooseList upgradeChooseList;

    // other stuff
    private Quaternion targetRotation;
    private bool isRotating;
    private int fireSalveCount;
    private float nextSalveTime = 0f;
    private float nextShotTime = 0f;
    private bool canFire = false;




    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        // find Objects
        player = GameObject.FindWithTag("Player");
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();

            // set start values from the weapon Controller
            // StartValues();

            //set first reload time
            nextSalveTime = Time.time + reloadSalveInterval;
    }

    private void Update()
    {
        if (isRotating)
        {
            // rotate the canon in the target direction and fire
            RotateTowardsTarget();
        }

        else
        {
            // find a new target
            FindNextTarget();
        }

        // fire salve control
        if (fireSalveCount >= fireSalveMax)
        {
            //reaload the head cannon after fire a salve
            if (Time.time >= nextSalveTime)
            {
                fireSalveCount = 0;
                nextSalveTime = Time.time + reloadSalveInterval;
            }
        }
        else
        {
            //reload after one shot
            if (Time.time >= nextShotTime)
            {
                canFire = true;
            }
        }

    }




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    private void StartValues()
    {
        PlayerWeaponController weaponController = player.GetComponent<PlayerWeaponController>();
        bulletDamage = weaponController.hcBulletDamage;
        fireSalveMax = weaponController.hcSalveCount;
        reloadSalveInterval = weaponController.hcReloadTime;
    }
        
    // find the nearst target
    private void FindNextTarget()
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagStr);
       
        float closestDistance = Mathf.Infinity;
        nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position); // Distanz zum Spielerobjekt berechnen

            // save the nearstEnemy
            if (distance <= detectionRange)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestEnemy != null)
        {
            CalculateTargetRotation();
        }
    }

    // set the rotation angle to target
    private void CalculateTargetRotation()
    {
        Vector3 targetDirection = nearestEnemy.transform.position - transform.position;
        targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        isRotating = true;
    }

    // rotate the headcannon to target and fire after rotate is complete
    private void RotateTowardsTarget()
    {
        // rotation to target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // rotation is complete, fire
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.05f)
        {
            ShotAferRotation();

            isRotating = false;   
        }
    }

    // trigger a bullet
    private void ShotAferRotation()
    {
        if (fireSalveCount < fireSalveMax && nearestEnemy != null && canFire == true)
        {
            int psIndex = 0;
            if (upgradeChooseList.upgrades[65].upgradeIndexInstalled > 0 || upgradeChooseList.upgrades[66].upgradeIndexInstalled > 0)
            {
                detectionRange = 12f;
                psIndex = 1;
            }

            // set damage to particle
            particleBullet[psIndex].bulletDamage = bulletDamage;

            // trigger the head cannon audio
            AudioManager.Instance.PlaySFX("PlayerHeadCannon");

            // trigger 1 particle shot
            bulletParticleSystem[psIndex].Emit(1);
            fireSalveCount ++;

            // reset timer
            nextSalveTime = Time.time + reloadSalveInterval;
            nextShotTime = Time.time + reloadShotInterval;
            canFire = false;
        }
    }
}
