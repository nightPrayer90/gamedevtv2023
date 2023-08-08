using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss2SidePhase : MonoBehaviour
{
    public Material emissive;
    public Material emissive_reverse;
    public Material building_reserve;

    public MeshRenderer meshRen;

    public Laser2 laser2;
    public ParticleSystem laserLoadParticle;
    public ParticleSystem shieldLoadParticle;
    public bool isZInverse = false;
    public bool isXTransform = false;

    public GameObject shield;
    public void ActivateWeapon(int mat, float delay)
    {
        
        switch(mat)
        {
            case 0:
                Invoke("InvokeActivateLaser", delay);
                break;

            case 1:
                Invoke("InvokeSpawnShield", delay);
                
                break;

        }
    }

    public void InvokeActivateLaser()
    {
        Material[] materials = meshRen.materials;
        AudioManager.Instance.PlaySFX("Boss2LaserLoad");
        materials[0] = emissive_reverse;
        materials[1] = building_reserve;

        if (isXTransform == false)
        {
            if (isZInverse == false)  { transform.DOLocalMoveZ(-0.1f, 2f); }
            else { transform.DOLocalMoveZ(0.1f, 2f); }
        }
        else
        {
            if (isZInverse == false) { transform.DOLocalMoveX(-0.1f, 2f); }
            else { transform.DOLocalMoveX(0.1f, 2f); }
        }


        Invoke("StartLaserShooting", 2f);
        meshRen.materials = materials;
        laserLoadParticle.Play();
    }

    public void StartLaserShooting()
    {
        AudioManager.Instance.PlaySFX("Boss2LaserFire");
        laser2.gameObject.SetActive(true);
        laser2.LaserActivate();
    }

    public void LaserDie()
    {
        laser2.LaserStop();
    }

    public void InvokeSpawnShield()
    {
        AudioManager.Instance.PlaySFX("Boss2SmallAlert");
        Material[] materials = meshRen.materials;
        materials[0] = emissive;
        meshRen.materials = materials;


        if (isXTransform == false)
        {
            if (isZInverse == false) { transform.DOLocalMoveZ(-0.2f, 2f); }
            else { transform.DOLocalMoveZ(0.2f, 2f); }
        }
        else
        {
            if (isZInverse == false) { transform.DOLocalMoveX(-0.2f, 2f); }
            else { transform.DOLocalMoveX(0.2f, 2f); }
        }

        AudioManager.Instance.PlaySFX("Boss2ShieldSpawn");
        Invoke("SpawnShield", 2);
        shieldLoadParticle.Play();
    }

    public void SpawnShield()
    {
        Instantiate(shield, transform.position, transform.rotation * Quaternion.Euler(0,270,0));

        if (isXTransform == false)
        {
            if (isZInverse == false) { transform.DOLocalMoveZ(0.1f, 0.4f).SetDelay(0.5f); }
            else { transform.DOLocalMoveZ(-0.1f, 0.4f).SetDelay(0.5f); ; }
        }
        else
        {
            if (isZInverse == false) { transform.DOLocalMoveX(0.1f, 0.4f).SetDelay(0.5f); ; }
            else { transform.DOLocalMoveX(-0.1f, 0.4f).SetDelay(0.5f); ; }
        }
    }

    public void ObjectDie()
    {
        Destroy(gameObject);
    }
}
