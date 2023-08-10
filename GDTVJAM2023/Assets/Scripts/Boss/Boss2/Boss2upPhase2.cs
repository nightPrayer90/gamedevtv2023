using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss2upPhase2 : MonoBehaviour
{
    public MeshRenderer mesh;
    public Laser2 laserPointer1;
    public ParticleSystem laserLoadParticle;
    public Boss2upPhase upPhase;


    public void ActivateMesh()
    {
        transform.DOLocalMoveY(0.08f, 2f);

        AudioManager.Instance.PlaySFX("Boss2SmallAlert");
        mesh.enabled = true;

        Invoke("StartShooting", 2f);
        laserLoadParticle.Play();
    }


    public void StartShooting()
    {
        AudioManager.Instance.PlaySFX("Boss2LaserFire");
        laserPointer1.gameObject.SetActive(true);
        laserPointer1.LaserActivate();
        upPhase.StartShooting();
    }

    public void LaserStop()
    {
        laserPointer1.LaserStop();
        upPhase.LaserStop();
        Destroy(gameObject);
    }
}
