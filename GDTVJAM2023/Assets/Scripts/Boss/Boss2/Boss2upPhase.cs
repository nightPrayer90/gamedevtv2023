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
    public float rotationSpeed = 0.5f;

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
    { mesh.enabled = true; }

    public void PhaseUP()
    {
        transform.DOMoveY(6.7f, 4).SetDelay(3f).OnComplete(() => {downPhase.ActivateShield();});
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
            transform.DOMoveY(9, 12f).SetDelay(2f).SetLoops(-1, LoopType.Yoyo);
        });
    }

    private void RotateUnit(float rotationSpeed)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
