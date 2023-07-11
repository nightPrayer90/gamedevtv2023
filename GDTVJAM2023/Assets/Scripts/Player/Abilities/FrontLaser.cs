using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FrontLaser : MonoBehaviour
{
    [Header("Bullet Particle")]
    public List<ParticleSystem> particleSystems;

    [Header("Weapon Settings")]
    public int bulletDamage = 10;
    public float laserShootTime = 1f;
    public float realodInterval = 5f;
    public float spawnInterval = 0.1f;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    private float laserDistance = 4;

    public LineRenderer lr;
    public ParticleSystem hitParticle;
    public ParticleSystem muzzleParticle;
    public bool laserIsEnable = false;
    private Color whiteZero = new Color(1f, 1f, 1f, 0f);
    private Color whiteStart = new Color(1f, 1f, 1f, 0.8f);
    private Color whiteEnd = new Color(1f, 1f, 1f, 0.3f);


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();

        // set damage to particle system
        foreach (ParticleSystem weapon in particleSystems)
        {
            weapon.GetComponent<ParticleBullet>().bulletDamage = bulletDamage;
        }

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

                // emit 1 particle of each weapon
                foreach (ParticleSystem weapon in particleSystems)
                {
                    if (weapon != null)
                        weapon.Emit(1);
                }
               
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
            GameObject collidedObject = hit.collider.gameObject;

            lr.SetPosition(1, collidedObject.transform.position);

            Vector3 dir = transform.position - collidedObject.transform.position;

            hitParticle.transform.position = collidedObject.transform.position + dir.normalized * .2f;
            if (!hitParticle.isPlaying)
                hitParticle.Play();
        }
        else
        {
            lr.SetPosition(1, transform.position + transform.forward * raycastDistance);
            hitParticle.Stop();
        }
    }

    void SetLRPosition()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + transform.forward * laserDistance);
    }

}
