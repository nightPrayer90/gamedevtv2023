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
    public GameObject gameOverUI;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UpdatePlayerHealth();
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
        expText.text = "Exp: " + player.playerCurrentExperience;
    }

}
