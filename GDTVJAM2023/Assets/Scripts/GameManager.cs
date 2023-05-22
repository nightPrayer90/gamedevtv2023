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

    public int[] selectedNumbers_ = new int[3];
    public int maxAbillitis = 20;

    public float curretEnemyCounter;
    public TextMeshProUGUI enemyCounterText;

    void Start()
    {

        currentTime = 0f;
        UpdateTimerText();

        player = GameObject.Find("Player").GetComponent<PlayerController>();
        UpdatePlayerHealth();
        UpdatePlayerExperience();
        UpdateEnemyCounter(0);
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

    public void UpdateEnemyCounter(float curretEnemyCounter_)
    {
        curretEnemyCounter = curretEnemyCounter + curretEnemyCounter_;
        enemyCounterText.text = "Enemys: " + curretEnemyCounter;
    }

    public void UpdatePlayerExperience()
    {

        // Levelup
        if (player.playerCurrentExperience == player.playerExperienceToLevelUp)
        {
            player.playerLevel += 1;
            player.playerExperienceToLevelUp = Mathf.RoundToInt(player.playerExperienceToLevelUp * player.playerLevelUpFactor);
            player.playerCurrentExperience = 0;

            int temphealth = Mathf.RoundToInt(player.playerMaxHealth * 0.25f);

            player.UpdatePlayerHealth(-temphealth);

            player.StopShooting();
            Time.timeScale = 0;

            CreateRandomNumbers();

            gameIsPlayed = false;

            panelUI.SetActive(true);
            playerUI.SetActive(false);
        }
        
        //Text
        expText.text = "Level: " + player.playerLevel + " Exp: " + player.playerCurrentExperience + "/" + player.playerExperienceToLevelUp;
        
    }


    private void CreateRandomNumbers()
    {
        // Array zum Speichern der ausgewählten Zahlen
        int[] selectedNumbers = new int[3];

        // Array zum Speichern aller verfügbaren Zahlen
        int[] availableNumbers = new int[maxAbillitis];
        for (int i = 0; i < availableNumbers.Length; i++)
        {
            availableNumbers[i] = i;
        }

        // Zufällige Auswahl von 3 Zahlen
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableNumbers.Length);

            // Ausgewählte Zahl zum ausgewählten Zahlen-Array hinzufügen
            selectedNumbers[i] = availableNumbers[randomIndex];

            // Ausgewählte Zahl aus dem verfügbaren Zahlen-Array entfernen
            availableNumbers[randomIndex] = availableNumbers[availableNumbers.Length - 1];
            System.Array.Resize(ref availableNumbers, availableNumbers.Length - 1);
        }

        // Ausgabe der ausgewählten Zahlen
        int a = 0;
        foreach (int number in selectedNumbers)
        {
            //Debug.Log("Ausgewählte Zahl: " + number);
            selectedNumbers_[a] = number;
            a++;
        }

        //Debug.Log("Ausgewählte Zahl: " + selectedNumbers_[0] + selectedNumbers_[1] + selectedNumbers_[2]);
    }

}
