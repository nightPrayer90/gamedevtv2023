using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Boss2Shield : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public int shieldHealth = 30;
    public List<ParticleCollisionEvent> collisionEvents; // creating a list to store the collision events

    public ParticleSystem rippleParticle;
    public ParticleSystem hitParticle;

    public MeshRenderer shieldMesh;
    private MeshCollider shieldCollider;


    private void Awake()
    {
       
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(1, 1, 1);
        
        Debug.Log(shieldCollider);

        transform.DOScale(150, 2f);
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);   
    }

    
    private void OnParticleCollision(GameObject other)
    {
        shieldHealth -= 1;

        ParticleSystem part = other.GetComponent<ParticleSystem>();
        int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
        {
            Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy
            hitParticle.transform.position = pos;
            hitParticle.Emit(30);
        }

       

        if (shieldHealth <= 0)
        {
            rippleParticle.Play();
            shieldCollider.GetComponent<MeshCollider>();
            shieldCollider.enabled = false;
            shieldMesh.enabled = false;
        }
        else
        {
            transform.DOComplete();
            transform.DOPunchScale(new Vector3(5f, 5f, 5f), 0.05f, 10, 1));
            
        }

       

    }
}
