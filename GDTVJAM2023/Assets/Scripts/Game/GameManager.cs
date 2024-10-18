using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    [Header("Game Status Control")]
    public bool gameIsPlayed = true;
    public bool gameOver = false;
    public bool isPause = false;
    public int districtNumber = 1;
    private int enemysToKill;
    private int killCounter;
    private int scrapCollected = 0;

    [Header("Spawn Status Control")]
    public int lootboxContainer;
    public int[] districtGroundEnemyControls = new int[9];

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
    public TextMeshProUGUI bossText;

    //public List<Color> globalClassColor;
    public ClassColor cCPrefab;
    public Transform outsideBorderTextTweenTarget;
    [HideInInspector] public float curretEnemyCounter;
    public GameObject miniMap;
    public CanvasGroup upgradeTextCG;
    public CanvasGroup minimapCG;
    public CanvasGroup minimapBigCG;
    public CanvasGroup playerUICG;
    public CanvasGroup controlsCG;
    public Camera minimapCameraBig;
    private bool bigMapisOpen = false;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI expGameOverText;
    public TextMeshProUGUI expGameVictoryText;
    public TextMeshProUGUI scrapText;
    public Image abBKImage;
    public Image abImage;
    public string abName = "";
    public float abReloadTime = 0;
    public GameObject abGlow;
    private float abValueBuffer = 0f;
    private float abTimeBuffer = 0f;
    private Tween abTween = null;



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
    public UpgradePanelController upgradePanelController;
    public UpgradeChooseList upgradeChooseList;
    public SpawnDistrictList spawnDistrictList;
    

    [Header("Floating Damage")]
    public GameObject textPrefab;


    [Header("Player Ship")]
    public PlayerData playerData;


    [Header("Objects")]
    public PlayerInputHandler inputHandler;
    public CameraShake topShake;
    public CameraShake backShake;
    public CinemachineSwitcher cinemachineSwitcher;
    public NewPlayerController player;
    // private PlayerWeaponController weaponController;
    public Light directionalLight;
    private GameObject currentSpawnManager;
    private bool isIntro = true;
    private bool canSpawnNextDimention = true;
    public MenuButtonController menuButtonController;
    public Postprocessing ppController;

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
        // Initialize timer
        currentTime = totalTime + 1;
        InvokeRepeating(nameof(UpdateTimerText), 3f, 1f);

        // Initialize Bgm
        AudioManager.Instance.PlayMusic("Dystrict1");

        // Set World Start Valus
        StartDimensionValues();

        // Initialze HUD Values
        UpdateUIPlayerExperience(false, 1, 10, 0, 0);
        UpdateTimerText();
        UpdateDistrictText();
        UpdateEnemyToKill(0);
        UpdateEnemyCounter(0);

        PlayerUIIntroTween();

        // Controlls - FadeIN and Out
        controlsCG.DOFade(1, 1f).SetDelay(0.5f).OnComplete(() => controlsCG.DOFade(0, 2f).SetDelay(12));

        lootboxContainer = 0;
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

        districtBaseShader.SetFloat("_DimensionControl", 1f);


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


    private void OnEnable()
    {
        // events
        inputHandler.OnOpenUIChanged += HandleBreakeUI;
        inputHandler.OnOpenMapInputChanged += HandleBigMinimapControl;
        inputHandler.OnHideUI += HideUI;
    }

    // only for us
    private void HideUI()
    {
        if (playerUICG.alpha == 1)
            playerUICG.alpha = 0.0f;
        else
            playerUICG.alpha = 1f;
    }

    private void HandleBigMinimapControl()
    {
        if (bigMapisOpen == false)
        {
            minimapCG.DOKill();
            minimapCG.alpha = 0;
            minimapBigCG.DOFade(1f, 0.2f);
            minimapCameraBig.enabled = true;
        }
        else
        {
            minimapBigCG.DOComplete();
            minimapCG.DOFade(1, 0.2f);
            minimapBigCG.DOKill();
            minimapBigCG.alpha = 0;
            minimapCameraBig.enabled = false;
        }
        bigMapisOpen = !bigMapisOpen;
    }


    // Open break UI
    private void HandleBreakeUI()
    {
        if (gameOver == false)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");

            inputHandler.OnOpenUIChanged -= HandleBreakeUI;

            inputHandler.DisableGameControls();
            inputHandler.EnableUIControls();
            PauseMenue();

            inputHandler.OnCloseUIChanged += HandleCloseBreakUI;
            inputHandler.OnClickInputChanged += HandleClickUI;
        }
    }

    // Open Close UI
    public void HandleCloseBreakUI()
    {

        inputHandler.OnCloseUIChanged -= HandleCloseBreakUI;
        inputHandler.OnClickInputChanged -= HandleClickUI;

        AudioManager.Instance.PlaySFX("MouseKlick");
        inputHandler.DisableUIControls();
        inputHandler.EnableGameControls();

        inputHandler.OnOpenUIChanged += HandleBreakeUI;

        PauseMenue();
    }

    public void HandleClickUI()
    {
        string selectedObject = EventSystem.current.currentSelectedGameObject.name;
        switch (selectedObject)
        {
            case "Back Button":
                Debug.Log("pressBackButton");
                HandleCloseBreakUI();
                break;
            case "Restart Button":
                AudioManager.Instance.PlaySFX("MouseKlick");
                inputHandler.DisableUIControls();
                inputHandler.EnableGameControls();
                menuButtonController.LevelRestart();
                break;
            case "BackToHangar Button":
                AudioManager.Instance.PlaySFX("MouseKlick");
                inputHandler.DisableUIControls();
                inputHandler.DisableGameControls();
                menuButtonController.BacktoHangar();
                break;
            case "BackToMenu Button":
                AudioManager.Instance.PlaySFX("MouseKlick");
                inputHandler.DisableUIControls();
                inputHandler.DisableGameControls();
                menuButtonController.BacktoMainMenue();
                break;
        }
    }

    private void OnDestroy()
    {
        // events
        inputHandler.OnOpenUIChanged -= HandleBreakeUI;
        inputHandler.OnCloseUIChanged -= HandleCloseBreakUI;
        inputHandler.OnClickInputChanged -= HandleClickUI;
        inputHandler.OnOpenMapInputChanged -= HandleBigMinimapControl;

        // Set game speed
        Time.timeScale = 1;

        // Reset dimension materials
        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        buildingMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_EmissionMap", secondDimenionTexture2);

        districtBaseShader.SetFloat("_DimensionControl", 1f);
    }

    /* **************************************************************************** */
    /* Manage Menu Panels---------------------------------------------------------- */
    /* **************************************************************************** */
    // activate gameoverUI
    public void GameIsOver()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        inputHandler.DisableGameControls();
        inputHandler.EnableUIControls();

        inputHandler.OnCloseUIChanged += HandleCloseBreakUI;
        inputHandler.OnClickInputChanged += HandleClickUI;

        Time.timeScale = 0;
        gameOver = true;
        gameIsPlayed = false;

        expGameOverText.text = CalculatePlayerEarnings(1f, false);

        gameOverUI.SetActive(true);
        playerUI.SetActive(false);
        bossUI.SetActive(false);
        victoryUI.SetActive(false);
    }

    // activate victoryUI
    public void Victory()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        inputHandler.DisableGameControls();
        inputHandler.EnableUIControls();

        inputHandler.OnCloseUIChanged += HandleCloseBreakUI;
        inputHandler.OnClickInputChanged += HandleClickUI;

        Time.timeScale = 0;
        gameOver = true;
        gameIsPlayed = false;

        expGameVictoryText.text = CalculatePlayerEarnings(1f, true);

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
        Invoke(nameof(InvokeupgradeText), 2f);

        gameIsPlayed = true;

        playerUI.SetActive(true);
        panelUI.SetActive(false);

        experienceSlider.DOValue(0, 0.4f, false);
        experienceSlider.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 5, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            experienceSlider.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        });
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

    public void TakeDamageEffekt()
    {
        ppController.TakeDamage();
    }

    // update the player experience bar - PlayerUI
    public void UpdateUIPlayerExperience(bool isLevelUp, int playerLevel, int playerExperienceToLevelUp, int playerCurrentExperience, int experienceGet)
    {
        experienceSlider.maxValue = playerExperienceToLevelUp;

        // Levelup
        if (isLevelUp)
        {
            // CompleteAllDestroy the Ability status - buffer the Value and set the tweener new after levelup 
            if (abTween != null)
            {
                abValueBuffer = abImage.fillAmount;
                abTimeBuffer = abTween.Duration() - abTween.Elapsed();

                DOTween.CompleteAll();

                if (abValueBuffer != 0)
                {
                    abImage.fillAmount = abValueBuffer;
                    SetAbilityValue(abTimeBuffer);
                }
            }
            else
            {
                DOTween.CompleteAll();
            }
            
            experienceSlider.value = experienceSlider.maxValue;
            upgradePanelController.CreateRandomNumbers(playerLevel);

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

        upgradePanelController.CreateRandomNumbers(-1);

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

                bossText.transform.localScale = new Vector3(1f, 1f, 1f);
                bossText.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.8f, 10, 0.5f).OnComplete(() =>
                {
                    bossText.transform.DOScale(new Vector3(0f, 0f, 0f), 0.15f).SetDelay(1.5f).OnPlay(() => AudioManager.Instance.PlaySFX("BossTextWoosh"));

                 });
                bossText.text = $"Boss District {districtNumber} is ready";
                AudioManager.Instance.PlaySFX("ShortAlert");
                

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

    public void UpdateTimer(float currentTime)
    {
        this.currentTime += currentTime;
        UpdateTimerText();
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
            CancelInvoke(nameof(UpdateTimerText));
        }
    }

    public void InitAbilityUI(Sprite abSprite, string abilityName, float reloadTime)
    {
        abBKImage.sprite = abSprite;
        abImage.sprite = abSprite;
        abName = abilityName; // container for stats menu
        abReloadTime = reloadTime; // container for stats menu

        Debug.Log("abilityName");
    }

    public void SetAbilityUItoZero()
    {
        abImage.DOKill();
        abImage.DOFillAmount(0, 0.1f);
        SetAbilityGlow(false);
    }

    public void SetAbilityGlow(bool isGlow)
    {
        abGlow.SetActive(isGlow);
    }

    public void SetAbilityValue(float relaodTime)
    {
        abImage.DOComplete();
        abTween = abImage.DOFillAmount(1, relaodTime).SetEase(Ease.Linear);
    }

    public void AddScrap(int scrap)
    {
        scrapCollected += scrap;
        AddScrapUI();
    }
    public void AddScrapUI()
    {
        scrapText.text = scrapCollected.ToString();
    }

    /* **************************************************************************** */
    /* Dimension swap ------------------------------------------------------------- */
    /* **************************************************************************** */
    // player goes into the second dimension
    public void GoToDimension()
    {
        // play boss bgm
        AudioManager.Instance.PlayMusic("BossMusic");
        ppController.GoToDimension();

        // swap materials
        buildingMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterial.SetTexture("_EmissionMap", secondDimenionTexture2);

        buildingMaterialReverse.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterialReverse.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterialReverse.SetTexture("_EmissionMap", firstDimensionTexture1);

        districtBaseShader.SetFloat("_DimensionControl", 0f);

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
        DestroyAllDamageFields();

        dimensionShift = true;
        OnDimensionSwap?.Invoke(dimensionShift);
    }

    // need short deelay for ppControllerEffekt
    public void GoBackDimensionInvoke()
    {
        Invoke(nameof(GoBackDimension), 0.1f);
    }


    // player goes back into the first dimension
    public void GoBackDimension()
    {
        ppController.GoToDimension();

        // swap materials
        buildingMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_MainTex", firstDimensionTexture1);
        emissionMaterial.SetTexture("_EmissionMap", firstDimensionTexture1);

        buildingMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_MainTex", secondDimenionTexture2);
        emissionMaterialReverse.SetTexture("_EmissionMap", secondDimenionTexture2);

        districtBaseShader.SetFloat("_DimensionControl", 1f);

        // set a new light color
        directionalLight.color = firstDimensionColor;

        // deactivate the boss UI
        bossUI.SetActive(false);

        // activate the next district 
        districtNumber++;
        Debug.Log(districtNumber);
        Invoke(nameof(UpdateDistrictText), 0.5f);

        // chance the bgm to the level bgm
        AudioManager.Instance.PlayMusic("Dystrict" + districtNumber.ToString());

        spawnDistrictList.districtList[districtNumber - 1].GrowUP();
        foreach (GroundBaseUp district in spawnDistrictList.districtList)
        {
            district.ChangeEmissiv(0, districtNumber);
        }

        AudioManager.Instance.PlaySFX("LiftUP");

        bossText.transform.localScale = new Vector3(1f, 1f, 1f);
        bossText.text = "A new District rises! \n Enemies get stronger.";
        bossText.transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.8f, 10, 0.5f).SetDelay(1f).OnPlay(()=> AudioManager.Instance.PlaySFX("ShortAlert")).OnComplete(() =>
        {
            bossText.transform.DOScale(new Vector3(0f, 0f, 0f), 0.15f).SetDelay(2f).OnPlay(() => AudioManager.Instance.PlaySFX("BossTextWoosh"));

        });

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

        Invoke(nameof(InvokeCanSpawnNext), 2f);
    }

    // (help function) destroy alle expOrbs if the player goes into the secound dimension
    public void DestroyAllEXPOrbs()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Exp");

        foreach (GameObject prefab in prefabs)
        {
            ObjectPoolManager.ReturnObjectToPool(prefab);
        }
    }

    public void DestroyAllDamageFields()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Player_BurningGround");

        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }
    }



    /* **************************************************************************** */
    /* MISC---------- ------------------------------------------------------------- */
    /* **************************************************************************** */

    // camera screenshake control
    public void ScreenShake(int shakeIndex)
    {
        switch (shakeIndex)
        {
            case 1:
                if (cinemachineSwitcher.topCamera_flag == true)
                    topShake.ShakeCamera(2f, 0.4f);
                else
                    backShake.ShakeCamera(0.3f, 0.4f);
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
                //else
                    //backShake.ShakeCamera(0, 0.0f);
                break;
            case 6:
                if (cinemachineSwitcher.topCamera_flag == true)
                    topShake.ShakeCamera(2, 1f);
                else
                    backShake.ShakeCamera(0.3f, 1f);
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
        timerTr.position = new Vector3(timerTr.position.x, timerTr.position.y + 100f, timerTr.position.z);
        districtTr.position = new Vector3(districtTr.position.x, districtTr.position.y + 100f, districtTr.position.z);
        killTextTr.position = new Vector3(killTextTr.position.x, killTextTr.position.y + 100f, killTextTr.position.z);
        miniMapTr.position = new Vector3(miniMapTr.position.x + 500, miniMapTr.position.y, miniMapTr.position.z);

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
    private string CalculatePlayerEarnings(float percent, bool isVictory = false)
    {
        string resultString = $"you collected {scrapCollected} Scraps";

        if (playerData.bossLevel < (districtNumber - 1)) //-1 because it starts with 1
        {
            resultString += "\n and unlock new modules in Module Shop!";
            playerData.bossLevel = districtNumber - 1;
        }

        playerData.credits += scrapCollected;

        AudioManager.Instance.SavePlayerData();

        return resultString;
    }
}


