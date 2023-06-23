using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWings : MonoBehaviour
{
    [Header("Bullet Particle")]
    public List<ParticleSystem> particleSystems;

    [Header("Weapon Settings")]
    public int bulletDamage = 10;
    public float realodInterval = 5f;
    public int salveMaxCount = 10;
    public float spawnInterval = 0.1f;
    public string audioClip = "";
    private float nextSpawnTime = 0f;
    private int salveCount = 0;




    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        StartValues();
        salveCount = salveMaxCount;
        
        foreach (ParticleSystem weapon in particleSystems)
        {
            weapon.GetComponent<ParticleBullet>().bulletDamage = bulletDamage;
        }

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
        bulletDamage = weaponController.bwDamage;
        salveMaxCount = weaponController.bwSalveCount;
        realodInterval = weaponController.bwRealoadTime;
    }

    // shooting controller
    void Shooting()
    {
        if (salveCount == salveMaxCount)
        {
            Invoke("RealodWeapon", realodInterval);
            salveCount++;
        }

        if (salveCount < salveMaxCount)
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
                        
                }

                salveCount++;
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    }

    // realod a salve of weapons
    void RealodWeapon()
    {
        salveCount = 0;
    }

}
