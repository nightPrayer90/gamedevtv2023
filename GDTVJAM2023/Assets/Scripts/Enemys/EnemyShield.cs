using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyShield : MonoBehaviour
{
    private GameObject spawnEnemy;
    public float rotationSpeed = 10f;
    public int shieldHealth = 30;
    public List<ParticleCollisionEvent> collisionEvents; // creating a list to store the collision events

    public ParticleSystem rippleParticle;
    public ParticleSystem hitParticle;

    public MeshRenderer shieldMesh;
    public MeshCollider shieldCollider;

    public GameObject replacement;
    private Transform playerTr;
    private Rigidbody playerRb;



    private void OnEnable()
    {
        //playerTr = GameObject.FindWithTag("Player").transform;
        //playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();

        transform.localScale = new Vector3(1, 1, 1);
        
        Debug.Log(shieldCollider);

        transform.DOScale(150, 2f);
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void Update()
    {
        if (playerTr == null)
        {
            playerTr = GameObject.FindWithTag("Player").transform;
            playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        }

        // spawnEnemy - todo
        transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);   
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX("Boss2ShieldBounce");

            hitParticle.transform.position = collision.transform.position;
            hitParticle.Emit(100);

            transform.DOComplete();
            transform.DOPunchScale(new Vector3(30f, 30f, 30f), 0.1f, 10, 1);

            PushThePlayerAway(15f);
        }
    }


    private void OnParticleCollision(GameObject other)
    {

        ParticleSystem part = other.GetComponent<ParticleSystem>();
        int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
        {
            Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy
            hitParticle.transform.position = pos;


        }

        ShieldGetDamage();
    }

    public void ShieldGetDamage(int damage = 1)
    {
        shieldHealth -= damage;

        if (shieldHealth <= 0)
        {
            AudioManager.Instance.PlaySFX("ShieldDie");

            ShieldDie();
        }
        else
        {
            AudioManager.Instance.PlaySFX("ImpactShot");

            ShieldGetHit();
        }
    }


    private void ShieldGetHit()
    {
        hitParticle.Emit(5);
        transform.DOComplete();
        transform.DOPunchScale(new Vector3(10f, 10f, 10f), 0.05f, 10, 1);
    }


    public void ShieldDie()
    {
        shieldCollider.enabled = false;
        transform.DOShakePosition(0.8f, 0.15f, 35, 90, false, false);
        transform.DOScale(new Vector3(140, 140, 140), 0.8f).OnComplete(() =>
        {
            rippleParticle.Play();
            PushThePlayer(3f, 10f);
            hitParticle.transform.position = gameObject.transform.position;
            hitParticle.Emit(80);
            AudioManager.Instance.PlaySFX("Boss2ShieldBounce");

            shieldMesh.enabled = false;
        });
        Instantiate(replacement, transform.position, transform.rotation);
 
    }


    private float DistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTr.transform.position);
        return distanceToPlayer;
    }

    private void PushThePlayer(float distance, float maxForce)
    {
        float distanceToPlayer = DistanceToPlayer();

        if (distanceToPlayer <= distance)
        {
            Vector3 pushDirection = playerTr.position - transform.position;
            Vector3 pushForceVector = pushDirection.normalized * maxForce;
            playerRb.AddForce(pushForceVector, ForceMode.Impulse);
        }
    }

    private void PushThePlayerAway(float forcepower)
    {
        Vector3 pushDirection = playerTr.position - transform.position;
        Vector3 pushForceVector = pushDirection.normalized * forcepower;
        playerRb.AddForce(pushForceVector, ForceMode.Impulse);
    }
}
