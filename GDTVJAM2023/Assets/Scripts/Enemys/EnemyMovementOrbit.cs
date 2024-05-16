using UnityEngine;
using System.Collections.Generic;

public class EnemyMovementOrbit : EnemyMovement
{
    [Header("Rotate Settings")]
    public float radius = 5f; // radius around the Player
    public float radiusRange = 1f; // radius Scatter
    public ParticleSystem strafeLeft;
    public ParticleSystem strafeRight;
    private int direction = 0; // 

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    protected override void Update()
    {
        if (!gameManager.dimensionShift)
        {
            // set state
            if (attackState == false)
            {
                FollowPlayer();
            }
            else
            {
                OrbitAroundPlayer();
            }
        }
    }

    protected override void HandleStateSwap(int state)
    {
        switch (state)
        {
            case 0:
                attackState = false;
                break;

            case 1:
                direction = Random.Range(0, 2);
                attackState = true;
                break;
        }
    }

    /* **************************************************************************** */
    /* Movement ------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region movement

    // movment - enemy flys around the player
    private void OrbitAroundPlayer()
    {
        Vector3 targetDirection = transform.position - playerTr.position;
        targetDirection.y = 0f; 

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        int dirFactor = 0;
        switch(direction)
        {
            case 0:
                dirFactor = 1;
                strafeRight.Emit(1);
                break;

            case 1:
                dirFactor = -1;
                strafeLeft.Emit(1);
                break;
        }

        Vector3 targetPosition = playerTr.position + (transform.right * radius * dirFactor);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
    #endregion
}
