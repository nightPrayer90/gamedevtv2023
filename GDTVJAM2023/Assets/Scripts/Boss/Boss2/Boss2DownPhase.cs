using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Boss2DownPhase : MonoBehaviour
{
    public GameObject replacement;
    public MeshRenderer shieldMesh;
    public MeshRenderer objMesh;

    private bool isRotate = false;
    public float rotationSpeed = 0.5f;

    public Collider shieldCollider1;

    private void Update()
    {
        if (isRotate == true)
        {
            RotateUnit(rotationSpeed);
        }
    }

    public void ActivateMesh()
    {
        objMesh.enabled = true;
    }

    public void GoOnPosition()
    {
        transform.DOMoveY(5f, 3f).SetDelay(2f);
    }

    public void ActivateShield()
    {
        shieldCollider1.enabled = true;
        isRotate = true;
        shieldMesh.enabled = true;
        shieldMesh.transform.DOScale(new Vector3(100, 100, 100), 0.5f);
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 2f, 10, 1);
    }

    private void RotateUnit(float rotationSpeed)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    public void ShieldDie()
    {
        Destroy(gameObject);
        Instantiate(replacement, transform.position, transform.rotation);
    }
}