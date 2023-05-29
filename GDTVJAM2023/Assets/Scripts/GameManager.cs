using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Status Controll")]
    public bool gameIsPlayed = true;
    public bool gameOver = false;
    public bool isPause = false;
    public int districtNumber = 1;
    private int enemysToKill;
    private int killCounter;
    private PlayerController player;
    [HideInInspector] public float curretEnemyCounter;
    private GameObject currentSpawnManager;
    private bool isGameOverUI_, isPlayerUI_, isBossUI_, isVictoryUI;

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
    public GameObject bossUI;
    public GameObject pauseUI;
    public GameObject victoryUI;
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
    public Material buildingMaterialReverse;
    public Material emissionMaterialReverse;
    public NavigationController navigationController;
    public Color firstDimensionColor;
    public Color secondDimenioncolor;
    private Light directionalLight;
    public GameObject ExpOrbDestroy;

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

        currentTime = totalTime;

        UpdateTimerText();
        UpdatePlayerHealth();
        UpdatePlayerExperience();
        UpdateEnemyCounter(0);
        UpdateDistrictText(districtNumber);
        UpdateEnemyToKill(0);
        StartDimentionSettings();

        Time.timeScale = 1;
        AudioManager.Instance.PlayMusic("InGameMusic");

        gameOverUI.SetActive(false);
        playerUI.SetActive(true);
        bossUI.SetActive(false);
        panelUI.SetActive(false);
        victoryUI.SetActive(false);
        pauseUI.SetActive(false);
    }

    public void StartDimentionSettings()
    {
        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        buildingMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_EmissionMap", secondDimenionTexture2);


    directionalLight.color = firstDimensionColor;

        currentSpawnManager = Instantiate(spawnDistrictList.spawnManagerList[districtNumber - 1], transform.position, transform.rotation);
    }


    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && gameIsPlayed)
        {
            PauseMenue();
        }

    }
    private void UpdateTimerText()
    {
        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 0)
        {
            GameIsOver();
        }
    }

    public void GameIsOver()
    {
        gameOver = true;
        gameIsPlayed = false;
        gameOverUI.SetActive(true);
        playerUI.SetActive(false);
        bossUI.SetActive(false);
        victoryUI.SetActive(false);
        Time.timeScale = 0;
    }

    public void UpdatePlayerHealth()
    {
        healthText.text = player.playerCurrentHealth + "/" + player.playerMaxHealth;

        healthBar.maxValue = player.playerMaxHealth;
        healthBar.value = player.playerCurrentHealth;
        

        if (player.playerCurrentHealth <= 0)
        {
            GameIsOver();
        }
    }

    public void UpdateEnemyCounter(float curretEnemyCounter_)
    {
        curretEnemyCounter = curretEnemyCounter + curretEnemyCounter_;
        enemyCounterText.text = "Enemys left: " + curretEnemyCounter;
    }

    public void UpdateDistrictText(int curretDistrict)
    {
        districtText.text = "District " + curretDistrict.ToString() + "/9";

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
            AudioManager.Instance.PlaySFX("LevelUp");

            player.playerLevel += 1;
            player.playerExperienceToLevelUp = Mathf.RoundToInt(player.playerExperienceToLevelUp * player.playerLevelUpFactor);
            player.playerCurrentExperience = 0;

            int temphealth = Mathf.RoundToInt(player.playerMaxHealth * 0.1f);

            player.UpdatePlayerHealth(-temphealth);

            player.StopShooting();
            player.engineAudioSource.Stop();
            Time.timeScale = 0;

            CreateRandomNumbers();

            gameIsPlayed = false;

            panelUI.SetActive(true);
            playerUI.SetActive(false);
            bossUI.SetActive(false);
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

        buildingMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_EmissionMap", secondDimenionTexture2);

        //boss UI
        bossUI.SetActive(false);

        // next Level
        districtNumber++;
        UpdateDistrictText(districtNumber-1);
        mainCamera.LongShakeScreen();
        directionalLight.color = firstDimensionColor;

        spawnDistrictList.districtList[districtNumber - 1].GetComponent<GroundBaseUp>().GrowUP();

        Instantiate(spawnDistrictList.spawnManagerList[districtNumber - 1], transform.position, transform.rotation);

        AudioManager.Instance.PlayMusic("InGameMusic");
        AudioManager.Instance.PlaySFX("LiftUP");
    }
    public void GoToDimension()
    {
        dimensionShift = true;

        buildingMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_EmissionMap", secondDimenionTexture2);

        buildingMaterialReverse.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterialReverse.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterialReverse.SetTexture("_EmissionMap", firstDimensionTexture1);

        mainCamera.BigShortShakeScreen();
        navigationController.DeactivateNavigatorMesh();
        directionalLight.color = secondDimenioncolor;

        Destroy(currentSpawnManager);
        DestroyAllEXPOrbs();

        AudioManager.Instance.PlayMusic("BossMusic");
    }


    public void CreateRandomNumbers()
    {
        //Auswahl der richtigen Liste
        if (((player.playerLevel % 5 + 3) == 0 || player.playerLevel == 3) && player.playerLevel <= 28)
         {
   
            valueList.AddRange(weaponChooseList.weaponIndex); // Greife auf die weaponIndex aus dem weaponChooseList zu
         }
         else
         {
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

    public void DestroyAllEXPOrbs()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag(ExpOrbDestroy.tag);

        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }
    }

    public void Victory()
    {
        //Game is over
        gameOver = true;
        gameIsPlayed = false;
        gameOverUI.SetActive(false);
        playerUI.SetActive(false);
        bossUI.SetActive(false);
        victoryUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void PauseMenue()
    {
        Debug.Log("Pause");

        if (isPause == false)
        {
            //status zwischenspeichern
            isGameOverUI_ = gameOverUI.activeSelf;
            isPlayerUI_ = playerUI.activeSelf;
            isBossUI_ = bossUI.activeSelf;
            isVictoryUI = victoryUI.activeSelf;
            pauseUI.SetActive(true);

            Time.timeScale = 0;
            isPause = true;
            AudioManager.Instance.PlaySFX("WindowOpen");
        }
        else
        {
            Time.timeScale = 1;
            isPause = false;

            //huds zurücksetzen
            gameOverUI.SetActive(isGameOverUI_);
            playerUI.SetActive(isPlayerUI_);
            bossUI.SetActive(isBossUI_);
            victoryUI.SetActive(isVictoryUI);

            pauseUI.SetActive(false);
            AudioManager.Instance.PlaySFX("MouseNo");
        }

    }

}
