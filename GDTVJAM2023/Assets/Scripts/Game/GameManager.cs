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
    [HideInInspector] public int districtNumber = 1;
    private int enemysToKill;
    private int killCounter;

    [Header("Time for one Round")]
    public float totalTime = 1800f;
    private float currentTime = 0f;

    [Header("UI Controll")]
    public GameObject gameOverUI;
    public GameObject panelUI;
    public GameObject playerUI;
    public GameObject bossUI;
    public GameObject pauseUI;
    public GameObject victoryUI;
    private bool isGameOverUI_, isPlayerUI_, isBossUI_, isVictoryUI;
    public Slider healthBar;
    public Slider experienceSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI districtText;
    public TextMeshProUGUI enemyToKillText;
    public TextMeshProUGUI enemyCounterText;
    public TextMeshProUGUI outsideBorderText;
    [HideInInspector] public float curretEnemyCounter;

    [Header("Dimension Shift")]
    public Texture firstDimensionTexture1;
    public Texture secondDimenionTexture2;
    public Material buildingMaterial;
    public Material emissionMaterial;
    public Material buildingMaterialReverse;
    public Material emissionMaterialReverse;
    public Color firstDimensionColor;
    public Color secondDimenionColor;
    [HideInInspector] public bool dimensionShift = false;

    //Listen für Abilitys und UpgradeSystem
    [Header("Upgrade Lists")]
    [HideInInspector] public WeaponChooseList weaponChooseList;
    [HideInInspector] public UpgradeChooseList upgradeChooseList;
    [HideInInspector] public SpawnDistrictList spawnDistrictList;
    [HideInInspector] public List<int> valueList;
    [HideInInspector] public int[] selectedNumbers_ = new int[3];
    
    // Objects
    private CameraController mainCamera;
    private PlayerController player;
    private Light directionalLight;
    private GameObject currentSpawnManager;

    public NavigationController navigationController;


    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        weaponChooseList = GetComponent<WeaponChooseList>();
        upgradeChooseList = GetComponent<UpgradeChooseList>();
        spawnDistrictList = GetComponent<SpawnDistrictList>();

        // Initialize timer
        currentTime = totalTime;
        InvokeRepeating("UpdateTimerText", 1f, 1f);

        // Initialize Bgm
        AudioManager.Instance.PlayMusic("InGameMusic");

        // Set World Start Valus
        StartDimensionValues();

        // Initialze HUD Values
        UpdateUIPlayerHealth(10,10);
        UpdateUIPlayerExperience(false, 1, 10, 0);
        UpdateEnemyCounter(0);
        UpdateDistrictText(districtNumber);
        UpdateEnemyToKill(0);

    }

    // sets start values that must be the same for every run
    public void StartDimensionValues()
    {
        // Set game speed
        Time.timeScale = 1;

        // Reset dimension materials
        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        buildingMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_EmissionMap", secondDimenionTexture2);

        // Reset directional ligth color
        directionalLight.color = firstDimensionColor;

        // instantiate the right Spawner
        currentSpawnManager = Instantiate(spawnDistrictList.spawnManagerList[districtNumber - 1], transform.position, transform.rotation);

        // set UI sights
        gameOverUI.SetActive(false);
        playerUI.SetActive(true);
        bossUI.SetActive(false);
        panelUI.SetActive(false);
        victoryUI.SetActive(false);
        pauseUI.SetActive(false);
    }

    private void Update()
    {
        // pause menu control
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && gameIsPlayed)
        {
            PauseMenue();
        }
    }




    /* **************************************************************************** */
    /* Manage Menu Panels---------------------------------------------------------- */
    /* **************************************************************************** */
    // activate gameoverUI
    public void GameIsOver()
    {
        Time.timeScale = 0;
        gameOver = true;
        gameIsPlayed = false;

        gameOverUI.SetActive(true);
        playerUI.SetActive(false);
        bossUI.SetActive(false);
        victoryUI.SetActive(false);
    }

    // activate victoryUI
    public void Victory()
    {
        Time.timeScale = 0;
        gameOver = true;
        gameIsPlayed = false;

        gameOverUI.SetActive(false);
        playerUI.SetActive(false);
        bossUI.SetActive(false);
        victoryUI.SetActive(true);
    }

    // activate pauseUI
    public void PauseMenue()
    {
        if (isPause == false)
        {
            Time.timeScale = 0;
            AudioManager.Instance.PlaySFX("WindowOpen");

            //status zwischenspeichern
            isGameOverUI_ = gameOverUI.activeSelf;
            isPlayerUI_ = playerUI.activeSelf;
            isBossUI_ = bossUI.activeSelf;
            isVictoryUI = victoryUI.activeSelf;
            pauseUI.SetActive(true);

            isPause = true;
        }
        else
        {
            Time.timeScale = 1;
            AudioManager.Instance.PlaySFX("MouseNo");
            

            //huds zurücksetzen
            gameOverUI.SetActive(isGameOverUI_);
            playerUI.SetActive(isPlayerUI_);
            bossUI.SetActive(isBossUI_);
            victoryUI.SetActive(isVictoryUI);
            pauseUI.SetActive(false);
            
            isPause = false;
        }
    }

    // deactivate upgradeUI and activate the playerUI
    public void UpgradeGet()
    { 
        Time.timeScale = 1;
        gameIsPlayed = true;

        playerUI.SetActive(true);
        panelUI.SetActive(false);
    }


    /* **************************************************************************** */
    /* Update PlayerUI------------------------------------------------------------- */
    /* **************************************************************************** */
    // update the player health UI bar - PlayerUI
    public void UpdateUIPlayerHealth(int playerCurrentHealth, int playerMaxHealth)
    {
        // update text
        healthText.text = playerCurrentHealth + "/" + playerMaxHealth;

        // update slider
        healthBar.maxValue = playerMaxHealth;
        healthBar.value = playerCurrentHealth;
        
        // check if is game over
        if (playerMaxHealth <= 0)
            GameIsOver();
    }

    // update the player experience bar - PlayerUI
    public void UpdateUIPlayerExperience(bool isLevelUp, int playerLevel, int playerExperienceToLevelUp, int playerCurrentExperience)
    {
        // Levelup
        if (isLevelUp)
        {
            CreateRandomNumbers(playerLevel);

            gameIsPlayed = false;

            panelUI.SetActive(true);
            playerUI.SetActive(false);
            bossUI.SetActive(false);

            expText.text = playerLevel.ToString();
        }

        experienceSlider.maxValue = playerExperienceToLevelUp;
        experienceSlider.value = playerCurrentExperience;
    }

    // update district text - PlayerUI
    public void UpdateDistrictText(int curretDistrict)
    {
        districtText.text = "District " + curretDistrict.ToString() + "/9";

        //wird bei jedem districtwechsel ausgelöst
        killCounter = 0;
        enemysToKill = spawnDistrictList.waveKillList[curretDistrict - 1];
    }

    // update the enemys to defeat text and spawn the dimension spawp items
    public void UpdateEnemyToKill(int amount)
    {
        killCounter = killCounter + amount;

        int enemyToDefeat = Mathf.Max(0, enemysToKill - killCounter);
        enemyToKillText.text = "Enemys to defeat: " + enemyToDefeat;

        // if the enemy kill quest for the current destrict is done
        if (enemyToDefeat == 0)
        {
            if (spawnDistrictList.goToDimensionPickup[districtNumber - 1].activeSelf == false)
            {
                // spawn the district dimension item
                spawnDistrictList.goToDimensionPickup[districtNumber - 1].SetActive(true);

                // activate the navigation controler
                navigationController.SetTargetPosition();

                // activate a camera shake
                mainCamera.BigShakeScreen();
            }
        }
    }

    // update the enemy counter text
    public void UpdateEnemyCounter(float curretEnemyCounter_)
    {
        curretEnemyCounter = curretEnemyCounter + curretEnemyCounter_;
        enemyCounterText.text = curretEnemyCounter.ToString();
    }

    // Update timer text - Invoke
    private void UpdateTimerText()
    {
        currentTime--;

        int minutes = (int)(currentTime / 60f);
        int seconds = (int)(currentTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 0)
        {
            GameIsOver();
            CancelInvoke("UpdateTimerText");
        }
    }



    // (help function) create 3 random numbers für the upgrade/ ability panel - trigger by levelup
    public void CreateRandomNumbers(int playerLevel)
    {
        List<int> selectedNumbers = new List<int>();

        // create temporary list from weapons or normal upgrades - depends on the player level
        if (((playerLevel % 4) == 3) && playerLevel <= 26)
            valueList.AddRange(weaponChooseList.weaponIndex);
        else
            valueList.AddRange(upgradeChooseList.upgradeIndex);

        // create 3 random possible numbers that do not duplicate each other
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, valueList.Count);     // generate a random number
            selectedNumbers.Add(valueList[randomIndex]);            // save the number in a list
            valueList.RemoveAt(randomIndex);                        // remove the value from the temp list
        }

        selectedNumbers_ = selectedNumbers.ToArray();

        //reset 
        valueList.Clear();
    }

    // (help function) an already installed weapon is removed from the weapons list - trigger by UpgradeWeaponController
    public void RemoveValueWeaponList(int removeIndex)
    {
        if (weaponChooseList != null)
        {
            int removePos = weaponChooseList.weaponIndex.IndexOf(removeIndex);

            weaponChooseList.weaponIndex.RemoveAt(removePos);
        }
    }




    /* **************************************************************************** */
    /* Dimension swap ------------------------------------------------------------- */
    /* **************************************************************************** */
    // player goes into the second dimension
    public void GoToDimension()
    {
        // play boss bgm
        AudioManager.Instance.PlayMusic("BossMusic");

        // swap materials
        buildingMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_EmissionMap", secondDimenionTexture2);

        buildingMaterialReverse.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterialReverse.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterialReverse.SetTexture("_EmissionMap", firstDimensionTexture1);

        // set a new light color
        directionalLight.color = secondDimenionColor;

        // screen shake
        mainCamera.BigShortShakeScreen();

        // deactivate player navigation controller (Control only over the player object)?
        navigationController.DeactivateNavigatorMesh();

        // destroy stuff
        Destroy(currentSpawnManager);
        DestroyAllEXPOrbs();

        dimensionShift = true;
    }

    // player goes back into the first dimension
    public void GoBackDimension()
    {
        // chance the bgm to the level bgm
        AudioManager.Instance.PlayMusic("InGameMusic");
        
        // swap materials
        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        buildingMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_EmissionMap", secondDimenionTexture2);

        // set a new light color
        directionalLight.color = firstDimensionColor;

        // deactivate the boss UI
        bossUI.SetActive(false);

        // activate the next district 
        districtNumber++;
        UpdateDistrictText(districtNumber);
        spawnDistrictList.districtList[districtNumber - 1].GetComponent<GroundBaseUp>().GrowUP();
        AudioManager.Instance.PlaySFX("LiftUP");

        // screen shake
        mainCamera.LongShakeScreen();

        // spawn a new SpawnManager
        Instantiate(spawnDistrictList.spawnManagerList[districtNumber - 1], transform.position, transform.rotation);

        dimensionShift = false;
    }

    // (help function) destroy alle expOrbs if the player goes into the secound dimension
    public void DestroyAllEXPOrbs()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Exp");

        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }
    }

}
