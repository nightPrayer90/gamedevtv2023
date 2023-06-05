using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeModul : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public float tickInterval = 2f;
    private float nextHealthTick = 12f;
    public float PercentHealthPerTick = 0.1f;
    private PlayerController playerController;
    public string audioClip = "";
    private GameManager gameManager;
    public Color healColor;

    // Update is called once per frame
    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        nextHealthTick = Time.time + tickInterval;
    }
    void Update()
    {
        if (playerController.playerCurrentHealth < playerController.playerMaxHealth)
        {
            if (Time.time >= nextHealthTick)
            {
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    particleSystem.Play();
                    
                }
                AudioManager.Instance.PlaySFX(audioClip);

                int heal = -Mathf.Max(1, Mathf.RoundToInt(playerController.playerMaxHealth * PercentHealthPerTick));

                gameManager.DoFloatingText(gameObject.transform.position, heal.ToString(), healColor);
                playerController.UpdatePlayerHealth(heal);
                nextHealthTick = Time.time + tickInterval;
            }
  
        }
    }
}
