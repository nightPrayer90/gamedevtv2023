using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;

public class EnemyMovementDash : EnemyMovement
{
    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDistance = 5f;
    public float dashLoadTime = 1.5f;
    private bool isDash = false;
    public ParticleSystem dashParticle;
    public GameObject dashMarker;
    public SpriteRenderer damageField;
    private bool dashMode = false;
    private bool isdamageField = false;

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
        damageField.DOKill();
        damageField.color = new Color32(255, 0, 55, 0);
        damageField.transform.localScale = new Vector3(0.1f, 0.5f, 0);
        dashMarker.SetActive(false);
        DeactivateBaseEngine();
        isDash = false;
        isdamageField = false;
        dashMode = false;
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
            if (isdamageField == false)
            {
                isdamageField = true;
                damageField.transform.DOScaleX(0.72f, dashLoadTime);
                damageField.DOColor(new Color32(255, 0, 55, 80), dashLoadTime).OnComplete(() =>
                {
                    Dash();
                });
            }
        }
        else
        {
            // Dash reset
            if (isDash == true && isdamageField == true)
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
                    isdamageField = false;
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


    private void Dash()
    {
        // Dash
        dashMarker.SetActive(true);
        Vector3 dashDirection = (playerTr.position - transform.position).normalized;
        enemyRb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        dashParticle.Play();
        engineParticle.Stop();
        Invoke(nameof(SetDashInvoke),0.1f);
        dashMode = true;
        damageField.color = new Color32(255, 0, 55, 0);
        damageField.transform.localScale = new Vector3(0.1f,0.5f,0);
    }

    private void SetDashInvoke()
    {
        isDash = true;
    }
    #endregion
}
