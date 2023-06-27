using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontLaser : MonoBehaviour
{
    [Header("Bullet Particle")]
    public List<ParticleSystem> particleSystems;

    [Header("Weapon Settings")]
    public int bulletDamage = 10;
    public float realodInterval = 5f;
    public int bulletMaxCount = 10;
    public float spawnInterval = 0.1f;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    private int bulletCount = 0;
    

    public LineRenderer lr;
    public ParticleSystem hitParticle;
    public ParticleSystem muzzleParticle;
    public bool laserIsEnable = false;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        bulletCount = bulletMaxCount;

        // set damage to particle system
        foreach (ParticleSystem weapon in particleSystems)
        {
            weapon.GetComponent<ParticleBullet>().bulletDamage = bulletDamage;
        }

    }

    // Update is called once per frame
    void Update()
    {
        SetLRPosition();
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
        bulletMaxCount = weaponController.flBulletCount;
        realodInterval = weaponController.flReloadTime;
    }

    // shooting controller
    void Shooting()
    {
        if (lr.enabled == true)
        {
            Raycast_();
        }

        if (bulletCount == bulletMaxCount)
        {
            Invoke("RealodWeapon", realodInterval);
            bulletCount++;
            lr.enabled = false;
            laserIsEnable = false;
            muzzleParticle.Stop();
        }

        if (bulletCount < bulletMaxCount)
        {
            if (Time.time >= nextSpawnTime)
            {
                // shooting sound
                AudioManager.Instance.PlaySFX(audioClip);

                // emit 1 particle of each weapon
                foreach (ParticleSystem weapon in particleSystems)
                {
                    if (weapon != null)
                        weapon.Emit(1);
                        bulletCount++;
                }
               
                nextSpawnTime = Time.time + spawnInterval;
            }
        }

    }

    void SetLRPosition()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + transform.forward * 5);
    }

    // realod a salve of weapons
    void RealodWeapon()
    {
        bulletCount = 0;
        lr.enabled = true;
        laserIsEnable = true;
        muzzleParticle.Play();
    }

    void Raycast_()
    {
        lr.SetPosition(0, transform.position);

        //lr.SetPosition(0, transform.position);
        int raycastDistance = 5; // Die maximale Entfernung des Raycasts
        int layerMask = (1 << 6) | (1 << 9); // Bitmaske für Render-Layer 6 und 8

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, layerMask))
        {
            // Kollision mit einem Objekt auf den gewünschten Render-Layern
            GameObject collidedObject = hit.collider.gameObject;
            //Debug.Log("Kollision mit " + collidedObject.name);

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

   
}
