using UnityEngine;
using System;

public class PickUps : MonoBehaviour
{
    public float moveSpeed = 1f;

    private Transform playerTransform;
    private bool ifcollect = false;

    public ParticleSystem pickUpParticle;
    public ParticleSystem OrbParticle;

    public event Action OnCollect;
    private float moveSpeedUp;

    [HideInInspector] public NewPlayerController playerController;
    [HideInInspector] public GameManager gameManager;
    public SphereCollider collectCollider;

    private void Awake()
    {
        var player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerTransform = player.GetComponent<Transform>();
            playerController = player.GetComponent<NewPlayerController>();
        }
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

    }

    void OnEnable()
    {
        //InvokeRepeating("DistanceToPlayer", 0, 0.2f);
        ifcollect = false;
        moveSpeedUp = 0;
        OrbParticle.gameObject.SetActive(true);
        collectCollider.enabled = false;
        Invoke(nameof(InvokeActivateCollider), 0.3f);
    }

    void Update()
    {
        if (ifcollect == true &&  moveSpeed > 0)
        {
            MoveToPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUpCollider"))
        {
            if (ifcollect == false)
                SetCollect();
        }
        if (other.CompareTag("Player"))
        {
            if (ifcollect == true)
                PickUpFX();
        }
    }

    private void InvokeActivateCollider()
    {
        collectCollider.enabled = true;
    }

    private void MoveToPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 newPosition = transform.position + directionToPlayer * (moveSpeed + moveSpeedUp) * Time.deltaTime;
        transform.position = newPosition;
        moveSpeedUp += 0.1f;
    }

    public void SetCollect()
    {
        ifcollect = true;
    }

    public void PickUpFX()
    {
        ifcollect = false;
        OnCollect?.Invoke();
        OrbParticle.gameObject.SetActive(false);
        if (pickUpParticle != null) pickUpParticle.Play();

        Invoke(nameof(GoBackToPool), 2.5f);
    }


    private void GoBackToPool()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
