using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Internal;

public class EnemyMovement : MonoBehaviour
{
    [Header("Base Movement Settings")]
    public GameObject tiltObject;
    public ParticleSystem engineParticle;
    public float speed;
    public float rotationSpeed;
    public bool canTilt = false;
    public float tiltBoarder = 20;
    

    //GameObjects
    protected bool attackState = false;
    protected float previousYRotation;
    protected float tiltAngle = 0;
    protected Transform playerTr;
    protected GameManager gameManager;
    protected Rigidbody enemyRb;
    protected EnemyPlayerDetector playerDetector;


    /* **************************************************************************** */
    /* LIFECYCLE AND EVENTS ------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle and Events
    protected void Awake()
    {
        playerDetector = gameObject.GetComponent<EnemyPlayerDetector>();
        playerDetector.OnSwapState += HandleStateSwap;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameManager.OnDimensionSwap += HandleDimensionSwap;

        enemyRb = GetComponent<Rigidbody>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    protected void OnEnable()
    {
        attackState = false;
        tiltAngle = 0;
        previousYRotation = 0;
        ActivateBaseEngine();
    }

    protected virtual void OnDisable()
    {
        DeactivateBaseEngine();
    }

    private void OnDestroy()
    {
        playerDetector.OnSwapState -= HandleStateSwap;
        gameManager.OnDimensionSwap -= HandleDimensionSwap;
    }

    protected virtual void Update()
    {
        if (!gameManager.dimensionShift)
        {
            FollowPlayer();
        }
    }

    protected virtual void HandleStateSwap(int state)
    {
        switch (state)
        {
            case 0:
                attackState = false;
                break;

            case 1:
                attackState = true;
                break;
        }
    }

    protected virtual void HandleDimensionSwap(bool isSecondDimension)
    {
        if (isSecondDimension == true)
        {
            DeactivateBaseEngine();
        }
        else
        {
            ActivateBaseEngine();
        }
    }
    #endregion



    /* **************************************************************************** */
    /* Movement ------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region movement

    // movement - basic follow the Player
    protected void FollowPlayer()
    {
        Vector3 directionToPlayer = playerTr.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        targetRotation *= Quaternion.Euler(0f, 180f, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 movement = directionToPlayer.normalized * speed * Time.deltaTime;
        transform.position += movement;

        TiltMesh();
    }

    protected void TiltMesh()
    {
        if (canTilt == true)
        {
            float currentYRotation = transform.rotation.eulerAngles.y;
            float angleDifference = Mathf.DeltaAngle(currentYRotation, previousYRotation);
            tiltAngle = Mathf.Lerp(tiltAngle, angleDifference, 1f * Time.deltaTime);
            float resultAngle = Mathf.Clamp(-tiltAngle * 60f, -tiltBoarder, tiltBoarder);
            tiltObject.transform.localRotation = Quaternion.Euler(0, 0, resultAngle);
            previousYRotation = currentYRotation;
        }
    }
    #endregion

    protected void ActivateBaseEngine()
    {
        if (engineParticle != null)
            engineParticle.Play();
    }

    protected void DeactivateBaseEngine()
    {
        if (engineParticle != null)
            engineParticle.Stop();
    }
}
