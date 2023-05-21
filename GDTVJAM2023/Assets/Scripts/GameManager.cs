using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool gameIsPlayed = true;
    public bool gameOver = false;
    private PlayerController player;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverUI;
    public GameObject panelUI;
    public GameObject playerUI;

    public float totalTime = 1800f;
    private float currentTime;

    void Start()
    {
        /*panelUI = GameObject.Find("LevelUpHud");
        playerUI = GameObject.Find("PlayerHud");
        gameOverUI = GameObject.Find("GameOverHud");*/

        currentTime = 0f;
        UpdateTimerText();

        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UpdatePlayerHealth();
        UpdatePlayerExperience();
    }

    private void Update()
    {
        if (currentTime < totalTime)
        {
            currentTime += Time.deltaTime;
            UpdateTimerText();
        }
    }
    private void UpdateTimerText()
    {
        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdatePlayerHealth()
    {
        healthText.text = "Health: " + player.playerCurrentHealth + "/" + player.playerMaxHealth;
        if (player.playerCurrentHealth <= 0)
        {
            gameOver = true;
            gameIsPlayed = false;
            gameOverUI.SetActive(true);
        }
    }

    public void UpdatePlayerExperience()
    {

        // Levelup
        if (player.playerCurrentExperience == player.playerExperienceToLevelUp)
        {
            player.playerLevel += 1;
            player.playerExperienceToLevelUp = Mathf.RoundToInt(player.playerExperienceToLevelUp * player.playerLevelUpFactor);
            player.playerCurrentExperience = 0;
            Time.timeScale = 0;
            
            
            gameIsPlayed = false;

            panelUI.SetActive(true);
            playerUI.SetActive(false);
        }
        else
        {
            //Text
            expText.text = "Level: " + player.playerLevel + " Exp: " + player.playerCurrentExperience + "/" + player.playerExperienceToLevelUp;
        }
    }

}
