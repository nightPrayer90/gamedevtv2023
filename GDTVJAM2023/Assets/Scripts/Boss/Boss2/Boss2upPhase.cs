using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss2upPhase : MonoBehaviour
{
    public MeshRenderer mesh;

    public Laser2 laserPointer1;
    public Laser2 laserPointer2;
    public Laser2 laserPointer3;
    public Laser2 laserPointer4;
    public Boss2DownPhase downPhase;
    public Collider objCollider;
    public GameObject replacement;

    private bool isRotate = false;
    public bool isShieldActive = false;
    public float rotationSpeed = -30f;

    private void Update()
    {
        if (isRotate == true)
        {
            RotateUnit(rotationSpeed);
        }
    }

    public void ActivateMesh()
    {
        Invoke("InvokeActivateMesh",1f); ;
    }

    public void InvokeActivateMesh()
    {
        AudioManager.Instance.PlaySFX("Boss2SmallAlert");
        mesh.enabled = true; 
    }

    public void PhaseUP()
    {
        AudioManager.Instance.PlaySFX("Boss2ShieldSpawn");
        transform.DOMoveY(6.9f, 2f).SetDelay(2f).OnComplete(() => 
        {
            isShieldActive = true;
            downPhase.ActivateShield();
            objCollider.enabled = true;
        });
    }

    public void StartShooting()
    {
        gameObject.transform.DOPunchScale(new Vector3(3f, 3f, 3f), 0.2f, 10, 1).OnComplete(()=> 
        {
            laserPointer1.gameObject.SetActive(true);
            laserPointer2.gameObject.SetActive(true);
            laserPointer3.gameObject.SetActive(true);
            laserPointer4.gameObject.SetActive(true);

            isRotate = true;
            transform.DOMoveY(7.9f, 7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        });
    }

    private void RotateUnit(float rotationSpeed)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void LaserStop()
    {
        laserPointer1.LaserStop();
        laserPointer2.LaserStop();
        laserPointer3.LaserStop();
        laserPointer4.LaserStop();
        Invoke("DieObject",0.3f);
    }

    private void DieObject()
    {
        Instantiate(replacement, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
