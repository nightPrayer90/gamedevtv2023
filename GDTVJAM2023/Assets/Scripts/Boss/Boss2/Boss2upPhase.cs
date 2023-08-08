using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss2upPhase : MonoBehaviour
{
    public MeshRenderer mesh;

    public GameObject laserPointer1;
    public GameObject laserPointer2;
    public GameObject laserPointer3;
    public GameObject laserPointer4;
    public Boss2DownPhase downPhase;

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
        transform.DOMoveY(6.7f, 2f).SetDelay(2f).OnComplete(() => 
        {
            isShieldActive = true;
            downPhase.ActivateShield();
        });
    }

    public void StartShooting()
    {
        gameObject.transform.DOPunchScale(new Vector3(3f, 3f, 3f), 0.8f, 10, 1).OnComplete(()=> 
        {
            laserPointer1.SetActive(true);
            laserPointer2.SetActive(true);
            laserPointer3.SetActive(true);
            laserPointer4.SetActive(true);

            isRotate = true;
            transform.DOMoveY(8.5f, 8f).SetDelay(1f).SetLoops(-1, LoopType.Yoyo);
        });
    }

    private void RotateUnit(float rotationSpeed)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
