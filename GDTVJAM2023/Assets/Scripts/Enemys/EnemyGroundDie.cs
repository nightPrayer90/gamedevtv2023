using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundDie : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private void Awake()
    {
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
    }
    private void OnEnable()
    {
        enemyHealth.DieEvent += OnDie;
        enemyHealth.gameManager.districtGroundEnemyControls[enemyHealth.gameManager.districtNumber] += 1;
    }

    private void OnDisable()
    {
        enemyHealth.DieEvent -= OnDie;
    }

    private void OnDie(object sender, EventArgs e)
    {
        enemyHealth.gameManager.districtGroundEnemyControls[enemyHealth.gameManager.districtNumber] -= 1;
    }
}
