using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Boss2Shield : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public int shieldHealth = 30;

    private void Awake()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnEnable()
    {
        transform.DOScale(150, 2f);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);   
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other);
        shieldHealth -= 1;
        transform.DOComplete();
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f, 10, 1);
    }
}
