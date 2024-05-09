using UnityEngine;
using System.Collections.Generic;

public class EnemyMovementDash : EnemyMovement
{
    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDistance = 5f;
    private bool isDash = false;
    public ParticleSystem dashParticle;
    public GameObject dashMarker;
    private bool dashMode = false;

    /* **************************************************************************** */
    /* LIFECYCLE AND EVENTS ------------------------------------------------------- */
    /* **************************************************************************** */
    #region LIFECYCLE AND EVENTS
    protected override void Update()
    {
        if (!gameManager.dimensionShift)
        {
            if (dashMode == false) FollowPlayer();
            DashToPlayer();
        }
    }

    protected override void OnDisable()
    {
        dashMarker.SetActive(false);
        DeactivateBaseEngine();
    }
    #endregion



    /* **************************************************************************** */
    /* Movement ------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region movement
    // movment - with dash  - Mass 2 - Drag 2 - force - 6.5 distance 2.7
    void DashToPlayer()
    {
        // Dash
        float distance = Vector2.Distance(playerTr.position, transform.position);
        if (distance < dashDistance && isDash == false)
        {
            // Dash
            dashMarker.SetActive(true);
            dashMode = true;
            Vector3 dashDirection = (playerTr.position - transform.position).normalized;
            enemyRb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            dashParticle.Play();
            engineParticle.Stop();
            isDash = true;
        }
        else
        {
            // Dash reset
            if (isDash == true)
            {
                if (enemyRb.velocity.magnitude <= 1.5f)
                {
                    dashMarker.SetActive(false);
                }
                if (enemyRb.velocity.magnitude <= 0.7f && dashMode == true)
                {
                    dashMode = false;
                    engineParticle.Play();
                }
                if (enemyRb.velocity.magnitude <= 0.3f)
                {
                    isDash = false;
                }

                // Rotation to player
                Vector3 directionToPlayer = playerTr.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2 * Time.deltaTime);

            }
        }
    }
    #endregion
}
