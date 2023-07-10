using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeModul : MonoBehaviour
{
    public float nextHealTick = 12f;
    public int healthPerTick = 1;
    public Color healColor;
    public ParticleSystem healParticleEffect;
    public string audioClip = "";
    private GameManager gameManager;
    private PlayerController playerController;
    

    // Update is called once per frame
    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        Invoke("Heal", nextHealTick);
    }
    void Heal()
    {
        if (playerController.playerCurrentHealth < playerController.playerMaxHealth)
        {
                healParticleEffect.Play();
                AudioManager.Instance.PlaySFX(audioClip);

                int heal = Mathf.Min(healthPerTick, playerController.playerMaxHealth - playerController.playerCurrentHealth);

                gameManager.DoFloatingText(gameObject.transform.position, "+" + heal.ToString(), healColor);
                playerController.UpdatePlayerHealth(-heal);
        }

        // trigger the next heal
        Invoke("Heal", nextHealTick);
    }
}
