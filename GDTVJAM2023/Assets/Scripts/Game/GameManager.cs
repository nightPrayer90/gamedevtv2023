using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;


public class GameManager : MonoBehaviour
{
    [Header("Game Status Controll")]
    public bool gameIsPlayed = true;
    public bool gameOver = false;
    public bool isPause = false;
    public int districtNumber = 1;
    private int enemysToKill;
    private int killCounter;
    private int expCollected;


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
    public Slider energieSlider;
    public Image boostFillArea;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI districtText;
    public TextMeshProUGUI enemyToKillText;
    public TextMeshProUGUI enemyCounterText;
    public TextMeshProUGUI outsideBorderText;
    //public List<Color> globalClassColor;
    public ClassColor cCPrefab;
    public Transform outsideBorderTextTweenTarget;
    [HideInInspector] public float curretEnemyCounter;
    public GameObject miniMap;
    public CanvasGroup upgradeTextCG;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI expGameOverText;
    public TextMeshProUGUI expGameVictoryText;


    [Header("Dimension Shift")]
    public Texture firstDimensionTexture1;
    public Texture secondDimenionTexture2;
    public Material buildingMaterial;
    public Material emissionMaterial;
    public Material buildingMaterialReverse;
    public Material emissionMaterialReverse;
    public Material districtBaseShader;
    public Color firstDimensionColor;
    public Color secondDimenionColor;
    [HideInInspector] public bool dimensionShift = false;
    public GameObject fogPlane;
    public GameObject fogPlaneDimension;


    //Listen für Abilitys und UpgradeSystem
    [Header("Upgrade Lists")]
    [HideInInspector] public UpgradeChooseList upgradeChooseList;
    [HideInInspector] public SpawnDistrictList spawnDistrictList;
    [HideInInspector] public List<int> valueList;
    [HideInInspector] public int[] selectedNumbers_ = new int[3];


    [Header("Floating Damage")]
    public GameObject textPrefab;

    [Header("Player Ship")]
    public PlayerData playerData;


    [Header("Objects")]
    public CameraShake topShake;
    public CameraShake backShake;
    public CinemachineSwitcher cinemachineSwitcher;
    public NewPlayerController player;
   // private PlayerWeaponController weaponController;
    private Light directionalLight;
    private GameObject currentSpawnManager;
    private bool isIntro = true;
    private bool canSpawnNextDimention = true;
    public MenuButtonController menuButtonController;


    // Events
    public event Action<bool> OnDimensionSwap;

    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        DOTween.KillAll();
    }

    void Start()
    {
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        upgradeChooseList = GetComponent<UpgradeChooseList>();
        spawnDistrictList = GetComponent<SpawnDistrictList>();

        // upgrades form all ships
        //upgradeChooseList.chanceToGetTwoExp = bulletShipData.chanceToDoubleExp + rocketShipData.chanceToDoubleExp + laserShipData.chanceToDoubleExp;

        // Initialize timer
        currentTime = totalTime + 1;
        InvokeRepeating("UpdateTimerText", 3f, 1f);

        // Initialize Bgm
        AudioManager.Instance.PlayMusic("Dystrict1");

        // Set World Start Valus
        StartDimensionValues();

        // Initialze HUD Values
        //UpdateUIPlayerHealth(10,10);
        UpdateUIPlayerExperience(false, 1, 10, 0, 0);
        UpdateTimerText();
        UpdateDistrictText();
        UpdateEnemyToKill(0);
        UpdateEnemyCounter(0);

        PlayerUIIntroTween();
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

        districtBaseShader.SetFloat("_DimensionControll", 1f);


        // Reset directional ligth color
        directionalLight.color = firstDimensionColor;

        // instantiate the right Spawner
        currentSpawnManager = Instantiate(spawnDistrictList.spawnManagerList[districtNumber - 1], transform.position, transform.rotation);
        currentSpawnManager.transform.SetParent(gameObject.transform);

        // set UI sights
        gameOverUI.SetActive(false);
        playerUI.SetActive(true);
        bossUI.SetActive(false);
        panelUI.SetActive(false);
        victoryUI.SetActive(false);
        pauseUI.SetActive(false);

        //FogPlanes
        fogPlane.SetActive(true);
        fogPlaneDimension.SetActive(false);
    }

    private void Update()
    {
        // pause menu control
        /*if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && gameIsPlayed)
        {
            PauseMenue();
        }*/

        // TODDOOO nur für die Messe
        if (gameOver == true)
        {
            if (Input.GetButtonDown("Boost"))
            {
                menuButtonController.LevelRestart();
            }
            if (Input.GetButtonDown("SwitchView"))
            {
                menuButtonController.BacktoMainMenue();
            }
        }
    }

    private void OnDestroy()
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

        districtBaseShader.SetFloat("_DimensionControll", 1f);
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

        expGameOverText.text = CalculateGlobalPlayerExp(1f, false);

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

        expGameVictoryText.text = CalculateGlobalPlayerExp(1f, true);

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
        upgradeText.text = upgradeChooseList.BuildlistCountAfterUpdate();

        // Fade Text in
        upgradeTextCG.DOFade(1, 0.5f);

        // Fade Text out
        Invoke("InvokeupgradeText", 2f);

        gameIsPlayed = true;

        playerUI.SetActive(true);
        panelUI.SetActive(false);

        experienceSlider.DOValue(0, 0.4f, false);
        experienceSlider.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 5, 0.5f).SetUpdate(true).OnComplete(() => {
            experienceSlider.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }); ;
    }

    private void InvokeupgradeText()
    {
        upgradeTextCG.DOFade(0, 0.5f);
    }


    /* **************************************************************************** */
    /* Update PlayerUI------------------------------------------------------------- */
    /* **************************************************************************** */
    // update the player health UI bar - PlayerUI
    public void UpdateUIPlayerHealth(int playerCurrentHealth, int playerMaxHealth)
    {
        if (isIntro == false)
        {
            // update text
            healthText.text = playerCurrentHealth + "/" + playerMaxHealth;

            // update slider
            healthBar.maxValue = playerMaxHealth;

            healthBar.DOValue(playerCurrentHealth, 0.5f);
            healthBar.transform.DOKill(true);
            healthBar.transform.DOShakePosition(0.5f, 2, 10, 90, true, true, ShakeRandomnessMode.Harmonic);


            // check if is game over
            if (playerMaxHealth <= 0)
                GameIsOver();
        }
    }

    // update the player experience bar - PlayerUI
    public void UpdateUIPlayerExperience(bool isLevelUp, int playerLevel, int playerExperienceToLevelUp, int playerCurrentExperience, int experienceGet)
    {
        experienceSlider.maxValue = playerExperienceToLevelUp;
        expCollected += experienceGet;
        
        // Levelup
        if (isLevelUp)
        {
            DOTween.CompleteAll();

            experienceSlider.value = experienceSlider.maxValue;
            CreateRandomNumbers(playerLevel);

            gameIsPlayed = false;

            panelUI.SetActive(true);
            playerUI.SetActive(false);
            bossUI.SetActive(false);

            expText.text = playerLevel.ToString();

        }
        else
        {
            if (isIntro == false)
            {
                experienceSlider.transform.DOKill(true);
                experienceSlider.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f, 5, 0.5f).SetUpdate(true);
            }
            experienceSlider.DOValue(playerCurrentExperience, 0.5f);
        }
    }

    // trigger if the player collect an Upgrade Pickup
    public void PlayerWeaponUpdatePickup() 
    {
        DOTween.CompleteAll();

        CreateRandomNumbers(-1);

        gameIsPlayed = false;

        panelUI.SetActive(true);
        playerUI.SetActive(false);
        bossUI.SetActive(false);

        // spawn the district dimension item
        //spawnDistrictList.goBackDimensionPickup[districtNumber - 1].SetActive(true);
    }

    // updade Player Energie slider
    public void UpdateEnergieSlider(float energieValue)
    {
        energieSlider.DOKill();
        energieSlider.DOValue(energieValue, 0.1f, false);
    }

    // update district text - PlayerUI
    public void UpdateDistrictText()
    {
        districtText.text = "District " + districtNumber.ToString() + "/9";
        if (isIntro == false)
            districtText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 1.5f, 5, 0.5f).SetUpdate(true);


        //wird bei jedem districtwechsel ausgelöst
        killCounter = 0;
        enemysToKill = spawnDistrictList.waveKillList[districtNumber - 1];
        enemyToKillText.text = "Enemies to defeat: " + enemysToKill.ToString();

        if (isIntro == false)
        {
            enemyToKillText.transform.DOKill(true);
            enemyToKillText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 1f, 5, 0.5f).SetUpdate(true);
        }
    }

    // update the enemys to defeat text and spawn the dimension spawp items
    public void UpdateEnemyToKill(int amount)
    {
        killCounter = killCounter + amount;

        int enemyToDefeat = Mathf.Max(0, enemysToKill - killCounter);
        enemyToKillText.text = "Enemies to defeat: " + enemyToDefeat;

        // if the enemy kill quest for the current destrict is done
        if (enemyToDefeat == 0 && canSpawnNextDimention == true)
        {
            if (spawnDistrictList.goToDimensionPickup[districtNumber - 1].activeSelf == false)
            {
                // spawn the district dimension item
                spawnDistrictList.goToDimensionPickup[districtNumber - 1].SetActive(true);
                canSpawnNextDimention = false;

                // activate the navigation controler
                player.SetNavigationController();

                // activate a camera shake
                ScreenShake(2);

                // tewwn enemytoDefeat text
                if (isIntro == false)
                {
                    enemyToKillText.transform.DOKill(true);
                    enemyToKillText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1.5f, 5, 0.5f).SetUpdate(true);
                }
            }
        }
    }

    private void InvokeCanSpawnNext()
    {
        canSpawnNextDimention = true;
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

        districtBaseShader.SetFloat("_DimensionControll", 0f);

        // set a new light color
        directionalLight.color = secondDimenionColor;
        foreach (GroundBaseUp district in spawnDistrictList.districtList)
        {
            district.ChangeEmissiv(1, districtNumber);
        }

        // screen shake
        ScreenShake(3);

        // set FogPlanes
        fogPlane.SetActive(false);
        fogPlaneDimension.SetActive(true);

        // destroy stuff
        Destroy(currentSpawnManager);
        DestroyAllEXPOrbs();

        dimensionShift = true;
        OnDimensionSwap?.Invoke(dimensionShift);
    }

    // player goes back into the first dimension
    public void GoBackDimension()
    {
        // swap materials
        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        buildingMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_EmissionMap", secondDimenionTexture2);

        districtBaseShader.SetFloat("_DimensionControll", 1f);

        // set a new light color
        directionalLight.color = firstDimensionColor;

        // deactivate the boss UI
        bossUI.SetActive(false);

        // activate the next district 
        //Debug.Log(districtNumber);
        districtNumber++;
        Debug.Log(districtNumber);
        Invoke("UpdateDistrictText", 0.5f);
        //UpdateDistrictText(districtNumber);

        // chance the bgm to the level bgm
        AudioManager.Instance.PlayMusic("Dystrict" + districtNumber.ToString());
        
        spawnDistrictList.districtList[districtNumber - 1].GrowUP();
        foreach (GroundBaseUp district in spawnDistrictList.districtList)
        {
            district.ChangeEmissiv(0, districtNumber);
        }

        AudioManager.Instance.PlaySFX("LiftUP");

        // screen shake
        ScreenShake(4);

        // set FogPlanes
        fogPlane.SetActive(true);
        fogPlaneDimension.SetActive(false);

        // spawn a new SpawnManager
        currentSpawnManager = Instantiate(spawnDistrictList.spawnManagerList[districtNumber - 1], transform.position, transform.rotation);
        currentSpawnManager.transform.SetParent(gameObject.transform);

        dimensionShift = false;
        OnDimensionSwap?.Invoke(dimensionShift);

        Invoke("InvokeCanSpawnNext", 2f);
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




    /* **************************************************************************** */
    /* MISC---------- ------------------------------------------------------------- */
    /* **************************************************************************** */

    // (help function) create 3 random numbers für the upgrade/ ability panel - trigger by levelup
    public void CreateRandomNumbers(int playerLevel)
    {
        List<int> selectedNumbers = new List<int>();

        // create temporary list from weapons or normal upgrades - depends on the player level
        if (playerLevel == -1) // new weapon
            valueList.AddRange(upgradeChooseList.BuildUpgradeList(Upgrade.UpgradeTyp.WeaponUpgrade)); 
        else if ((playerLevel % 5) == 0) // class update
            valueList.AddRange(upgradeChooseList.BuildUpgradeList(Upgrade.UpgradeTyp.ClassUpgrade));
        else
            valueList.AddRange((upgradeChooseList.BuildUpgradeList(Upgrade.UpgradeTyp.NormalUpgrade)));

        // create 3 random possible numbers that do not duplicate each other
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, valueList.Count);     // generate a random number
            selectedNumbers.Add(valueList[randomIndex]);            // save the number in a list
            valueList.RemoveAt(randomIndex);                        // remove the value from the temp list
        }
        
        selectedNumbers_ = selectedNumbers.ToArray();

        //reset 
        valueList.Clear();
    }

    // camera screenshake control
    public void ScreenShake(int shakeIndex)
    {
        switch(shakeIndex)
        {
            case 1:
                if (cinemachineSwitcher.topCamera_flag == true)
                    topShake.ShakeCamera(1f, 0.3f);
                else
                    backShake.ShakeCamera(0.3f, 0.3f);
                break;

            case 2:
                if (cinemachineSwitcher.topCamera_flag == true)
                    topShake.ShakeCamera(2f, 0.5f);
                else
                    backShake.ShakeCamera(0.3f, 0.5f);
                break;

            case 3:
                if (cinemachineSwitcher.topCamera_flag == true)
                    topShake.ShakeCamera(1f, 1f);
                else
                    backShake.ShakeCamera(0.3f, 1f);
                break;

            case 4:
                if (cinemachineSwitcher.topCamera_flag == true)
                    topShake.ShakeCamera(0.4f, 10f);
                else
                    backShake.ShakeCamera(0.25f, 10f);
                break;

            case 5:
                if (cinemachineSwitcher.topCamera_flag == true)
                    topShake.ShakeCamera(0.3f, 0.2f);
                else
                    backShake.ShakeCamera(0.1f, 0.2f);
                break;
        }
    }

    // create a floating text after calculation damage or health
    public void DoFloatingText(Vector3 position, string text, Color c)
    {
        GameObject floatingText = ObjectPoolManager.SpawnObject(textPrefab, position, Quaternion.LookRotation(Camera.main.transform.forward), ObjectPoolManager.PoolType.FloatingText);
        floatingText.GetComponent<TMP_Text>().color = c;
        // You can call this from anywhere by calling gameManager.DoFloatingText(position, text, c);
        floatingText.GetComponent<DamagePopup>().SetText(text);
    }
    
    public void PlayerUIIntroTween()
    {
        healthText.text = "";
        healthBar.value = 0;

        // save default position from the Editor
        Transform timerTr = timerText.transform;
        Transform districtTr = districtText.transform;
        Transform killTextTr = enemyToKillText.transform;
        Transform miniMapTr = miniMap.transform;
        float miniX = miniMapTr.position.x;

        // set textmesh outsinde from view
        timerTr.position = new Vector3(timerTr.position.x, timerTr.position.y+100f, timerTr.position.z);
        districtTr.position = new Vector3(districtTr.position.x, districtTr.position.y + 100f, districtTr.position.z);
        killTextTr.position = new Vector3(killTextTr.position.x, killTextTr.position.y + 100f, killTextTr.position.z);
        miniMapTr.position = new Vector3(miniMapTr.position.x+500, miniMapTr.position.y, miniMapTr.position.z);

        //AudioManager.Instance.PlaySFX("WindowOpen");
        experienceSlider.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.4f, 5, 1).SetDelay(0.2f).OnComplete(() =>
        {
           AudioManager.Instance.PlaySFX("PlayerLaserDie");

           healthBar.maxValue = player.playerMaxHealth;
           energieSlider.maxValue = player.energieMax;
           healthBar.DOValue(healthBar.maxValue, 0.6f, false).SetEase(Ease.InExpo).OnComplete(() => 
            { 
                healthText.text = player.playerCurrentHealth + "/" + player.playerMaxHealth;
            });
            healthBar.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 5, 1).SetDelay(0.55f);
        }); 


        // doTween elements
        timerTr.DOLocalMoveY(-14f, .3f, true).SetUpdate(UpdateType.Normal, true).SetDelay(1.4f).OnComplete(() =>
        {
            timerTr.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.25f, 5, 1);
            AudioManager.Instance.PlaySFX("MouseKlick");
        });
        districtTr.DOLocalMoveY(-14f, .3f, true).SetUpdate(UpdateType.Normal, true).SetDelay(1.8f).OnComplete(() =>
        {
            districtTr.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.25f, 5, 1);
            AudioManager.Instance.PlaySFX("MouseKlick");
        });
        killTextTr.DOLocalMoveY(-14f, .3f, true).SetUpdate(UpdateType.Normal, true).SetDelay(2.2f).OnComplete(() =>
        {
            killTextTr.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.25f, 5, 1);
            AudioManager.Instance.PlaySFX("MouseKlick");
            
            miniMapTr.DOMoveX(miniX, .25f, true).SetUpdate(UpdateType.Normal, true).SetDelay(0.2f).OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("MouseKlick");
                miniMapTr.DOPunchScale(new Vector3(0.12f, 0.12f, 0.12f), 0.25f, 5, 1);
                //AudioManager.Instance.PlaySFX("WindowOpen");
                isIntro = false;
            });
        });
    }

    // write earned exp in playerData
    private string CalculateGlobalPlayerExp(float percent, bool isVictory = false)
    {
        expCollected = Mathf.RoundToInt((float)expCollected * percent);

        string resultString = $"you earned {expCollected} Credits";

        if (playerData.bossLevel < (districtNumber - 1)) //-1 because it starts with 1
        {
            resultString += "\n and unlock new modules in Module Shop!";
            playerData.bossLevel = districtNumber - 1;
        }

        //TODO: Only for the DEMO
        if (isVictory == true)
            playerData.bossLevel = 5;

        playerData.credits += expCollected;

        AudioManager.Instance.SavePlayerData();

        return resultString;
    }
}


