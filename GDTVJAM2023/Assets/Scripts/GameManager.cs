using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Status Controll")]
    public bool gameIsPlayed = true;
    public bool gameOver = false;
    public int districtNumber = 1;
    private int enemysToKill;
    private int killCounter;
    private PlayerController player;
    [HideInInspector] public float curretEnemyCounter;

    [Header("UI Controll")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI districtText;
    public TextMeshProUGUI enemyToKillText;
    public TextMeshProUGUI enemyCounterText;
    public GameObject gameOverUI;
    public GameObject panelUI;
    public GameObject playerUI;
    public Slider healthBar;
    public Slider experienceSlider;

    [Header("Time for one Round")]
    public float totalTime = 1800f;
    private float currentTime = 0f;

    [Header("Dimension Shift")]
    public bool dimensionShift = false;
    private CameraController mainCamera;
    public Texture firstDimensionTexture1;
    public Texture secondDimenionTexture2;
    public Material buildingMaterial;
    public Material emissionMaterial;
    public NavigationController navigationController;
    public Color firstDimensionColor;
    public Color secondDimenioncolor;
    private Light directionalLight;

    //Listen für Abilitys und UpgradeSystem
    [Header("Upgrade Lists")]
    public WeaponChooseList weaponChooseList;
    public UpgradeChooseList upgradeChooseList;
    public SpawnDistrictList spawnDistrictList;
    [HideInInspector] public List<int> valueList;
    [HideInInspector] public int[] selectedNumbers_ = new int[3];

    void Start()
    {

        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        //navigationController = GameObject.Find("Navigator Controller").GetComponent<NavigationController>();

        UpdateTimerText();
        UpdatePlayerHealth();
        UpdatePlayerExperience();
        UpdateEnemyCounter(0);
        UpdateDistrictText(districtNumber);
        UpdateEnemyToKill(0);
        StartDimentionSettings();
    }

    public void StartDimentionSettings()
    {
        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        directionalLight.color = firstDimensionColor;
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
        healthText.text = player.playerCurrentHealth + "/" + player.playerMaxHealth;

        healthBar.maxValue = player.playerMaxHealth;
        healthBar.value = player.playerCurrentHealth;


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
        enemyCounterText.text = "Enemys left: " + curretEnemyCounter;
    }

    public void UpdateDistrictText(int curretDistrict)
    {
        enemyCounterText.text = "District" + curretDistrict + "/9";

        //wird bei jedem districtwechsel ausgelöst
        killCounter = 0;
        enemysToKill = spawnDistrictList.waveKillList[curretDistrict - 1];
    }

    //Spawn des Dimensionsitems
    public void UpdateEnemyToKill(int amount)
    {
        killCounter = killCounter + amount;

        int enemyToDefeat = Mathf.Max(0, enemysToKill - killCounter);
        enemyToKillText.text = "Enemys to defeat: " + enemyToDefeat;

        //Spawn des Dimensionsitems
        if (enemyToDefeat == 0)
        {
            if (spawnDistrictList.goToDimensionPickup[districtNumber - 1].activeSelf == false)
            {
                spawnDistrictList.goToDimensionPickup[districtNumber - 1].SetActive(true);

                navigationController.SetTargetPosition();

                mainCamera.BigShakeScreen();
            }
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

            int temphealth = Mathf.RoundToInt(player.playerMaxHealth * 0.25f);

            player.UpdatePlayerHealth(-temphealth);

            player.StopShooting();
            Time.timeScale = 0;

            CreateRandomNumbers();

            gameIsPlayed = false;

            panelUI.SetActive(true);
            playerUI.SetActive(false);
        }

        experienceSlider.maxValue = player.playerExperienceToLevelUp;
        experienceSlider.value = player.playerCurrentExperience;
        //Text
        expText.text = player.playerLevel.ToString();

    }

    
    public void GoBackDimension()
    {
        dimensionShift = false;

        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        // next Level
        districtNumber++;
        UpdateDistrictText(districtNumber);
        mainCamera.LongShakeScreen();
        directionalLight.color = firstDimensionColor;

        spawnDistrictList.districtList[districtNumber - 1].GetComponent<GroundBaseUp>().GrowUP();
    }
    public void GoToDimension()
    {
        dimensionShift = true;

        buildingMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_EmissionMap", secondDimenionTexture2);

        mainCamera.BigShortShakeScreen();
        navigationController.DeactivateNavigatorMesh();
        directionalLight.color = secondDimenioncolor;
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

    public void Victory()
    {
        //Game is over

    }



}
