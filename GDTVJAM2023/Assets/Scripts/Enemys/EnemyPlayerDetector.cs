using System;
using UnityEngine;

public class EnemyPlayerDetector : MonoBehaviour
{
    [HideInInspector] public Rigidbody playerRb;
    [HideInInspector] private float distanceToPlayer;
    public float attackDistance = 7;
    private float attacDistanceWithScatter = 0;
    public float distanceScatter = 1;
    public float updateTime = 0.25f;
    private int lastState = -1;
    private GameManager gameManager;

    // Events
    public event Action<int> OnSwapState;



    /* **************************************************************************** */
    /* LIFECYCLE AND EVENTS ------------------------------------------------------- */
    /* **************************************************************************** */
    #region LIFECYCLE AND EVENTS
    private void Awake()
    {
        playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(SetAttackState), 0f, 0.25f);
        distanceToPlayer = 0;
        lastState = -1;
        attacDistanceWithScatter = attackDistance + UnityEngine.Random.Range(-distanceScatter, distanceScatter);
        gameManager.OnDimensionSwap += HandleDimensionSwap;
    }

    private void OnDisable()
    {
        gameManager.OnDimensionSwap -= HandleDimensionSwap;
        CancelInvoke();
    }

    private void HandleDimensionSwap(bool isSecondDimension)
    {
        if (isSecondDimension == true)
        {
            CancelInvoke(nameof(SetAttackState));
        }
        else
        {
            InvokeRepeating(nameof(SetAttackState), 0f, updateTime);
            distanceToPlayer = 0;
            lastState = -1;
        }
    }
    #endregion


    /* **************************************************************************** */
    /* PLAYERDETECTION ------------------------------------------------------------ */
    /* **************************************************************************** */
    #region PLAYERDETECTION

    private void SetAttackState()
    {
        distanceToPlayer = DistanceToPlayer();
        int state = 0;

       
        if (distanceToPlayer < attacDistanceWithScatter)
        {
            state = 1;
        }

        if (state != lastState)
        {
            OnSwapState?.Invoke(state); // 0=move state  1=attack state
            lastState = state;
        }
    }

    public float DistanceToPlayer()
    {
        Vector2 playerPosition = new Vector2(playerRb.transform.position.x, playerRb.transform.position.z);
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);

        float distance = Vector2.Distance(playerPosition, currentPosition);

        return distance;
    }
    #endregion
}
