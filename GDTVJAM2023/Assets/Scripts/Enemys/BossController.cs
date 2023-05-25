using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    private GameObject player;
    private GameManager gameManager;
    public EnemyHealth enemyHealth;
    public float activationRange = 10f;
    public float moveSpeed = 5f;
    public float targetYPosition = 6f;
    public MonoBehaviour battleScriptPlaceholder;
    private CameraController cameraController;
    public GameObject bossHud;
    public Slider bossHealthSlider;

    private bool isActivated = false;
    private bool isHealthUpdated = false;

    private void Start()
    {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        //enemyHealth = gameObject.GetComponent<EnemyHealth>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.Find("Player");

        if (battleScriptPlaceholder != null)
        {
            battleScriptPlaceholder.enabled = false;
        }
    }
    private void Update()
    {
        if (!isActivated && gameManager.dimensionShift == true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= activationRange)
            {
                ActivateBoss();
            }

            
        }
        if (isActivated &&  isHealthUpdated == true)
        {
            bossHealthSlider.value = enemyHealth.enemyHealth;
        }
    }

    private void ActivateBoss()
    {
        isActivated = true;
        cameraController.ShakeScreen();

        // Bewege den Boss zur Zielposition
        StartCoroutine(MoveToTargetPosition());
    }

    private IEnumerator MoveToTargetPosition()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, targetYPosition, transform.position.z);
        float remainingDistance = Vector3.Distance(transform.position, targetPosition);

        while (remainingDistance > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            remainingDistance = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }

        StartBossBattle();
    }

    private void StartBossBattle()
    {
        enemyHealth.canTakeDamage = true;
     
        if (battleScriptPlaceholder != null)
        {
            battleScriptPlaceholder.enabled = true; 
        }
        bossHud.SetActive(true);
        bossHealthSlider.maxValue = enemyHealth.enemyHealth;
        bossHealthSlider.value = enemyHealth.enemyHealth;
        isHealthUpdated = true;
    }
}