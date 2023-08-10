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
    public MeshRenderer shieldEmitterMesh;
    public Collider shieldCollider;
    public Collider backCollider;

    public GameObject replacement;
    private Transform playerTr;
    private Rigidbody playerRb;
    private GameManager gameManager;
    public Color hitColor;
    public Rigidbody rb;
    private bool isdied = false;

    private void OnEnable()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if(player != null)
        {
            playerTr = player.transform;
            playerRb = player.GetComponent<Rigidbody>();
        }

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        transform.localScale = new Vector3(1, 1, 1);

        transform.DOScale(1, 2f);
        collisionEvents = new List<ParticleCollisionEvent>();
    }


    private void Update()
    {
        if (playerTr == null) // todoo
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
            transform.DOPunchScale(new Vector3(0.12f, 0.12f, 0.12f), 0.2f, 10, 1);

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
            gameManager.DoFloatingText(pos, "+1", hitColor);
        }

        ShieldGetDamage();
    }

    public void ShieldGetDamage(int damage = 1)
    {
        if (isdied == false)
        {
            shieldHealth -= damage;

            if (shieldHealth <= 0)
            {
                AudioManager.Instance.PlaySFX("ShieldDie");

                isdied = true;
                ShieldDie();
            }
            else
            {
                AudioManager.Instance.PlaySFX("ImpactShot");

                ShieldGetHit();
            }
        }
    }

    public void ActivateShield()
    {
        shieldMesh.enabled = true;
        shieldCollider.enabled = true;
        shieldMesh.transform.DOScale(new Vector3(100, 100, 100), 0.5f);
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 2f, 10, 1);
    }

    private void ShieldGetHit()
    {
        hitParticle.Emit(5);
        transform.DOComplete();
        transform.DOPunchScale(new Vector3(.05f, .05f, .05f), 0.05f, 10, 1);
    }

    public void ShieldDie()
    {
        transform.DOShakePosition(1f, 0.15f, 35, 90, false, false);
        transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 1f).OnComplete(() =>
        {
            Instantiate(replacement, transform.position, transform.rotation);
            shieldCollider.enabled = false;
            backCollider.enabled = false;
            rippleParticle.Play();
            PushThePlayer(3f, 10f);
            hitParticle.transform.position = gameObject.transform.position;
            hitParticle.Emit(80);
            AudioManager.Instance.PlaySFX("Boss2ShieldBounce");
            Invoke("ShielDelete", 8f);

            shieldMesh.enabled = false;
            gameObject.tag = "Untagged";
            if (shieldEmitterMesh != null) shieldEmitterMesh.enabled = false;
        });
    }

    private void ShielDelete()
    {
        Destroy(gameObject);
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
