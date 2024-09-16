using UnityEngine;
using System;

public class PickUps : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float detectionRange = 0.5f;
    [HideInInspector] public GameManager gameManager;

    [HideInInspector] public NewPlayerController playerController;
    private Transform playerTransform;
    private bool ifcollect = false;

    public ParticleSystem pickUpParticle;
    public ParticleSystem OrbParticle;

    public event Action OnCollect;
    private float moveSpeedUp;

    private void Awake()
    {
        var player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerController = player.GetComponent<NewPlayerController>();
            playerTransform = player.GetComponent<Transform>();
        }
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void OnEnable()
    {
        InvokeRepeating("DistanceToPlayer", 0, 0.2f);
        ifcollect = false;
        moveSpeedUp = 0;
        OrbParticle.gameObject.SetActive(true);
    }

    void Update()
    {
        if (ifcollect == true)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToPlayer * (moveSpeed+ moveSpeedUp) * Time.deltaTime;
            transform.position = newPosition;
        }
    }

    private void DistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        detectionRange = playerController.pickupRange;

        if (distanceToPlayer < detectionRange)
        {
            ifcollect = true;
        }

        if (ifcollect == true)
        {
            moveSpeedUp += 0.1f;
        }

        if (distanceToPlayer < 0.5f)
        {
            PickUpFX();
        }
    }

    public void SetCollect()
    {
        moveSpeedUp = 4;
        ifcollect = true;
    }

    public void PickUpFX()
    {
        OnCollect?.Invoke();
        OrbParticle.gameObject.SetActive(false);
        pickUpParticle.Play();
        CancelInvoke("DistanceToPlayer");

        Invoke(nameof(GoBackToPool), 2.5f);
    }


    private void GoBackToPool()
    {
        Debug.Log("GoBackTOPOOL");
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
