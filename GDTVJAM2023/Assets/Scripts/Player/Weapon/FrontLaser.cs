using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FrontLaser : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int bulletDamage = 10;
    public float laserShootTime = 1f;
    public float realodInterval = 5f;
    public string audioClip = "";
    private float laserDistance = 4;
    [HideInInspector] public bool laserIsEnable = false;
    private float nextSpawnTime = 0f;
    private float spawnInterval = 3f;

    [Header("Weapon Objects")]
    public LineRenderer lr;
    public ParticleSystem hitParticle;
    public ParticleSystem muzzleParticle;
    public ParticleSystem collisionParticle;
    private Color whiteZero = new Color(1f, 1f, 1f, 0f);
    private Color whiteStart = new Color(1f, 1f, 1f, 0.8f);
    private Color whiteEnd = new Color(1f, 1f, 1f, 0.3f);
    private GameManager gameManager;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // start fireing
        StopLaserShooting();

        lr.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        Shooting();
    }




    /* **************************************************************************** */
    /* RUNTIME METHODEN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    private void StartValues()
    {
        PlayerWeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        bulletDamage = weaponController.flDamage;
        laserShootTime = weaponController.flShootingTime;
        realodInterval = weaponController.flReloadTime;
    }

    // shooting controller
    void Shooting()
    {
        SetLRPosition();

        if (laserIsEnable == true)
        {
            Raycast_();

            if (Time.time >= nextSpawnTime)
            {
                // shooting sound
                AudioManager.Instance.PlaySFX(audioClip);
                nextSpawnTime = Time.time + spawnInterval;
            }
        }

    }

    void StartLaserShooting()
    {
        Invoke("StopLaserShooting", laserShootTime);

        muzzleParticle.Play();

        // FadeIn
        lr.DOColor(new Color2(whiteZero, whiteZero), new Color2(whiteStart, whiteEnd), 0.8f).SetEase(Ease.OutElastic).OnComplete(() =>
        { Invoke("LaserFadeOut", laserShootTime - 1.5f); });

        lr.enabled = true;
        laserIsEnable = true;
    }

    private void LaserFadeOut()
    {
        lr.DOColor(new Color2(whiteStart, whiteEnd), new Color2(whiteZero, whiteZero), 1f).SetEase(Ease.InBounce);
    }

    private void StopLaserShooting()
    {
        Invoke("StartLaserShooting", realodInterval);

        muzzleParticle.Stop();
        laserIsEnable = false;
    }

    void Raycast_()
    {
        float raycastDistance = laserDistance;
        int layerMask = (1 << 6) | (1 << 9);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, layerMask))
        {
            EnemyHealth collidedObject = hit.collider.gameObject.GetComponent<EnemyHealth>();

            if (collidedObject != null)
            {

                if (collidedObject.canTakeLaserDamage[3] == true && collidedObject.canTakeDamage == true)
                {
                    collidedObject.TakeLaserDamage(bulletDamage, 3);
                    collidedObject.ShowDamageFromPosition(hit.point, bulletDamage);
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
            lr.SetPosition(1, transform.position + transform.forward * raycastDistance);
        }
    }

    void SetLRPosition()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + transform.forward * laserDistance);
    }

}
