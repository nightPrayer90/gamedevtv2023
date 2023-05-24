using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Status Controll")]
    public bool gameIsPlayed = true;
    public bool gameOver = false;
    private PlayerController player;

    [Header("UI Controll")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverUI;
    public GameObject panelUI;
    public GameObject playerUI;

    [Header("Time for one Round")]
    public float totalTime = 1800f;
    private float currentTime = 0f;
    public TextMeshProUGUI enemyCounterText;
    [HideInInspector] public float curretEnemyCounter;

    [Header("Dimension Shift")]
    public bool dimensionShift = false;
    public Texture firstDimensionTexture1;
    public Texture secondDimenionTexture2;
    public Material buildingMaterial;
    public Material emissionMaterial;

    //Listen für Abilitys und UpgradeSystem
    [Header("Upgrade Lists")]
    public WeaponChooseList weaponChooseList;
    public UpgradeChooseList upgradeChooseList;
    [HideInInspector] public List<int> valueList;
    [HideInInspector] public int[] selectedNumbers_ = new int[3];

    void Start()
    {
        //weaponChooseList = FindObjectOfType<WeaponChooseList>();
        //upgradeChooseList = FindObjectOfType<UpgradeChooseList>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        
        UpdateTimerText();

       
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

    public void DimensionShift()
    {
        if (dimensionShift == true)
        {
            dimensionShift = false;

            buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
            emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
            emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        }
        else
        {
            dimensionShift = true;

            buildingMaterial.SetTexture("_MainTex", secondDimenionTexture2);
            emissionMaterial.SetTexture("_MainTex", secondDimenionTexture2);
            emissionMaterial.SetTexture("_EmissionMap", secondDimenionTexture2);
        }
    }


    public void CreateRandomNumbers()
    {
        //Auswahl der richtigen Liste
        if (player.playerLevel % 2 == 0)
         {
            Debug.Log("test1");
            valueList.AddRange(weaponChooseList.weaponIndex); // Greife auf die weaponIndex aus dem weaponChooseList zu
         }
         else
         {
            Debug.Log("test2");
            valueList.AddRange(upgradeChooseList.upgradeIndex); // Greife auf die upgradeIndex aus dem upgradeChooseList zu
         }
       
        List<int> selectedNumbers = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            // Generiere eine zufällige Indexposition
            int randomIndex = Random.Range(0, valueList.Count);

            // Übertrage den Wert an der zufälligen Position in die ausgewählten Zahlen
            selectedNumbers.Add(valueList[randomIndex]);
            
            // Entferne den ausgewählten Wert aus der Liste, um ihn nicht erneut auszuwählen
            valueList.RemoveAt(randomIndex);
        }
        
        // *Hinweis: Es werden wirklich nur Werte übergeben und keine Positionen
        selectedNumbers_ = selectedNumbers.ToArray();
        //Debug.Log("Ausgewählte Zahl: " + selectedNumbers_[0] + " " + selectedNumbers_[1] + " " + selectedNumbers_[2]);

        //reset 
        valueList.Clear();
    }


    public void RemoveValueWeaponList(int removeIndex)
    {
        if (weaponChooseList != null)
        {
            int removePos = weaponChooseList.weaponIndex.IndexOf(removeIndex);

            weaponChooseList.weaponIndex.RemoveAt(removePos);
        }
    }



}
