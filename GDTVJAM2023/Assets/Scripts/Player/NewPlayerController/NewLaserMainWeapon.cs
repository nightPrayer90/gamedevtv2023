using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLaserMainWeapon : MonoBehaviour
{
    [Header("Main Weapon")]
    public int bulletBaseDamage;
    public float fireRate;

    [Header("Laser Settings")]
    public float spawnInterval = 0.1f;
    public float laserRange = 5f;
    public float laserShootTime = 3f;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    public LineRenderer lr;
    public ParticleSystem hitParticle;
    public ParticleSystem muzzleParticle;
    public ParticleSystem collisionParticle;
    public bool laserIsEnable = false;
    public Transform LaserSpawnPoint1;
    private Color whiteZero = new Color(1f, 1f, 1f, 0f);
    private Color whiteStart = new Color(1f, 1f, 1f, 0.8f);
    private Color whiteEnd = new Color(1f, 1f, 1f, 0.3f);

    //private Objects
    private GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.gameObject.GetComponent<UpgradeChooseList>();
        laserShootTime = 3f; //shipData.laserShootingTime;

        lr.enabled = false;
        Invoke("StartLaserShooting", 3);
    }

    private void Update()
    {
        LaserShooting();
    }


    /* **************************************************************************** */
    /* LASER ---------------------------------------------------------------------- */
    /* **************************************************************************** */
    // shooting controller
    void LaserShooting()
    {
        SetLaserLRPosition();

        if (laserIsEnable == true)
        {
            LaserRaycast();

            if (Time.time >= nextSpawnTime)
            {
                // shooting sound
                AudioManager.Instance.PlaySFX(audioClip);
                nextSpawnTime = Time.time + spawnInterval;
            }

        }
    }

    private void StopLaserShooting()
    {
        Invoke("StartLaserShooting", fireRate);

        muzzleParticle.Stop();

        laserIsEnable = false;
    }

    // realod a salve of weapons
    void StartLaserShooting()
    {
        Invoke("StopLaserShooting", laserShootTime);

        muzzleParticle.Play();

        lr.enabled = true;

        // FadeIn
        lr.DOColor(new Color2(whiteZero, whiteZero), new Color2(whiteStart, whiteEnd), 0.8f).SetEase(Ease.OutElastic).OnComplete(() =>
        { Invoke("LaserFadeOut", laserShootTime - 1.5f); });

        laserIsEnable = true;
    }

    private void LaserFadeOut()
    {
        lr.DOColor(new Color2(whiteStart, whiteEnd), new Color2(whiteZero, whiteZero), 1f).SetEase(Ease.InBounce);
    }

    private void LaserRaycast()
    {
        float raycastDistance = laserRange;
        int layerMask = (1 << 6) | (1 << 9);

        // laser 1
        RaycastHit hit;
        if (Physics.Raycast(LaserSpawnPoint1.position, -LaserSpawnPoint1.forward, out hit, raycastDistance, layerMask))
        {
            EnemyHealth collidedObject = hit.collider.gameObject.GetComponent<EnemyHealth>();

            if (collidedObject != null)
            {

                if (collidedObject.canTakeLaserDamage[1] == true && collidedObject.canTakeDamage == true)
                {
                    collidedObject.TakeLaserDamage(bulletBaseDamage, 1);
                    collidedObject.ShowDamageFromPosition(hit.point, bulletBaseDamage);
                    collisionParticle.transform.position = hit.point;
                    collisionParticle.Play();
                }
            }
            else
            {
                EnemyShield collidedShield = hit.collider.GetComponentInParent<EnemyShield>();
                if (collidedShield != null)
                {
                    if (collidedShield.canTakeDamage == true)
                    {
                        collidedShield.ShieldGetLaserDamage();
                        collidedShield.ShowDamageFromPosition(hit.point);
                        collisionParticle.transform.position = hit.point;
                        collisionParticle.Play();
                    }
                }

            }

            lr.SetPosition(1, hit.point);

            hitParticle.transform.position = hit.point;
            hitParticle.Emit(1);
        }
        else
        {
            lr.SetPosition(1, LaserSpawnPoint1.position - LaserSpawnPoint1.forward * raycastDistance);
        }
    }

    private void SetLaserLRPosition()
    {
        lr.SetPosition(0, LaserSpawnPoint1.position);
        lr.SetPosition(1, LaserSpawnPoint1.position - LaserSpawnPoint1.forward * laserRange);
    }
}
