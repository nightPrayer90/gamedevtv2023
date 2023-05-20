using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool gameIsPlayed = true;
    private PlayerController player;
    public TextMeshProUGUI healthText;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UpdatePlayerHealth();
    }

    public void UpdatePlayerHealth()
    {
        healthText.text = "Health: " + player.playerCurrentHealth + "/" + player.playerMaxHealth;
    }
}
