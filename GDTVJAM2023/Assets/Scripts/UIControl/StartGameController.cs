using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;



public class StartGameController : MonoBehaviour
{
    [Header("Menue Panel Handler")]
    public CanvasGroup gameStartPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup creditsPanel;

    [Header("Start Game Panel")]
    public Transform loadUITransform;
    public CanvasGroup loadUICanvasGroup;
    private bool loadUIFlag = false;

    public List<Sprite> districts;
    public List<Image> districtIamges;
    public List<TextMeshProUGUI> durationTexts;
    public List<TextMeshProUGUI> creditsTexts;
    public List<TextMeshProUGUI> districtTexts;
    public List<MainMenuLoadPanel> loadPanel;
    public List<GameObject> deleteBtn;

    private IDataService DataService = new JsonDataService();
    private bool encriptionEnabled = false;

    public List<PlayerStats> playerStats;


    [Header("Settings Panel")]
    public Slider _musicSlider;
    public Slider _sfxSlider;

    public TMP_Dropdown resolutionDropdrown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullScreenBtn;
    private float _musicVolume;
    private float _sfxVolume;
    private bool isSettingsOpen = false;
    Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    private int currentResolutionIndex = 0;
    private float currentRefreshRate;

    [Header("Credits Panel")]
    private bool isCreditssOpen = false;

    public InputActionReference Action;
    public InputActionAsset AllTheActions;
    InputActionRebindingExtensions.RebindingOperation rebindOperation;

    private void Start()
    {
        Time.timeScale = 1;
        HandleStartSettings();

        gameStartPanel.alpha = 1f;
        gameStartPanel.blocksRaycasts = true;
        settingsPanel.alpha = 0f;
        settingsPanel.blocksRaycasts = false;
        creditsPanel.alpha = 0;
        creditsPanel.blocksRaycasts = false;

        loadUITransform.localPosition = new Vector3(0, 900f, 0);

        loadUICanvasGroup.alpha = 0;
        loadUICanvasGroup.blocksRaycasts = false;

        LoadPlayerData();
    }

    private void Update()
    {
        // Fade out
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (loadUIFlag == true)
                FadeLoadUIOut();

            if (isSettingsOpen == true)
                CloseSettings();

            if (isCreditssOpen == true)
                CloseCredits();
        }*/
    }


    /* **************************************************************************** */
    /* Handle Navigation ---------------------------------------------------------- */
    /* **************************************************************************** */

    public void OpenSettings()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        isSettingsOpen = true;

        gameStartPanel.DOComplete();
        gameStartPanel.DOFade(0, 0.3f);
        gameStartPanel.blocksRaycasts = false;

        settingsPanel.DOComplete();
        settingsPanel.DOFade(1, 0.3f);
        settingsPanel.blocksRaycasts = true;
    }

    public void CloseSettings()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        isSettingsOpen = false;

        gameStartPanel.DOComplete();
        gameStartPanel.DOFade(1, 0.3f);
        gameStartPanel.blocksRaycasts = true;

        settingsPanel.DOComplete();
        settingsPanel.DOFade(0, 0.3f);
        settingsPanel.blocksRaycasts = false;
    }

    public void OpenCredits()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        isCreditssOpen = true;

        gameStartPanel.DOComplete();
        gameStartPanel.DOFade(0, 0.3f);
        gameStartPanel.blocksRaycasts = false;

        creditsPanel.DOComplete();
        creditsPanel.DOFade(1, 0.3f);
        creditsPanel.blocksRaycasts = true;
    }

    public void CloseCredits()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        isCreditssOpen = false;

        gameStartPanel.DOComplete();
        gameStartPanel.DOFade(1, 0.3f);
        gameStartPanel.blocksRaycasts = true;

        creditsPanel.DOComplete();
        creditsPanel.DOFade(0, 0.3f);
        creditsPanel.blocksRaycasts = false;
    }

    /* **************************************************************************** */
    /* Start Game Stuff ----------------------------------------------------------- */
    /* **************************************************************************** */
    #region start game stuff
    public void FadeLoadUIIn()
    {
        if (loadUIFlag == false)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");
            loadUIFlag = true;
            loadUICanvasGroup.DOComplete();
            loadUITransform.DOComplete();
            loadUICanvasGroup.blocksRaycasts = true;
            loadUICanvasGroup.DOFade(1, 0.2f).SetUpdate(true);
            loadUITransform.DOLocalMoveY(0, 0.4f).SetUpdate(true);
        }
    }

    private void FadeLoadUIOut()
    {
        if (loadUIFlag == true)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");
            loadUIFlag = false;
            loadUICanvasGroup.blocksRaycasts = false;
            loadUICanvasGroup.DOComplete();
            loadUITransform.DOComplete();
            loadUITransform.DOLocalMoveY(-900, 0.2f).SetUpdate(true);
            loadUICanvasGroup.DOFade(0, 0.3f).SetUpdate(true).OnComplete(() => loadUITransform.DOLocalMoveY(900, 0.01f).SetUpdate(true));
        }
    }

    private void LoadPlayerData()
    {
        String path = "";

        for (int i = 0; i < 3; i++)
        {
            // load Player Data
            playerStats[i] = new();
            path = $"playerData{i + 1}.json";
            playerStats[i] = DataService.LoadData<PlayerStats>(path, encriptionEnabled);

            // update Panel
            UpdateLoadPanel(i);
        }
    }

    private void UpdateLoadPanel(int i)
    {
        // no Data to Load - new Game
        if (playerStats[i].playerName == "default-player")
        {
            districtIamges[i].gameObject.SetActive(false);
            loadPanel[i].canLoad = false;
            deleteBtn[i].SetActive(false);
        }
        // Data to Load - load Game
        else
        {
            districtIamges[i].sprite = districts[playerStats[i].bossLevel];
            creditsTexts[i].text = $"{playerStats[i].credits} CD";
            districtTexts[i].text = playerStats[i].bossLevel.ToString();
            loadPanel[i].canLoad = true;
            deleteBtn[i].SetActive(true);
        }
    }

    public void CreateNewGame(int loadIndex)
    {
        Debug.Log("create New Game");
        AudioManager.Instance.SetPlayerDataToDefault($"playerData{loadIndex}.json");
        AudioManager.Instance.SceneTransition("IntroScene");
    }

    public void LoadGame(int loadIndex)
    {
        Debug.Log("Laod Game");
        AudioManager.Instance.LoadPlayerData($"playerData{loadIndex}.json");
        AudioManager.Instance.SceneTransition("HangarScene");
    }

    public void DeleteProfile(int index)
    {
        Debug.Log("Delete Game");
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + $"playerData{index}.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        PlayerStats newplayerStats = new();

        playerStats[index - 1] = newplayerStats;

        UpdateLoadPanel(index - 1);
    }
    #endregion

    public void GameQuit()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        Application.Quit();
    }

    /* **************************************************************************** */
    /* Settings -------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region settings
    private void HandleStartSettings()
    {
        if (_musicSlider != null)
        {
            _musicVolume = AudioManager.Instance.musicVolume;
            _sfxVolume = AudioManager.Instance.sfxVolume;

            AudioManager.Instance.MusicVolume(_musicVolume);
            AudioManager.Instance.SFXVolume(_sfxVolume);

            SetSlider();
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        if (fullScreenBtn != null)
        {
            fullScreenBtn.isOn = Screen.fullScreen;
        }

        if (resolutionDropdrown != null)
        {
            resolutions = Screen.resolutions;
            filteredResolutions = new List<Resolution>();

            resolutionDropdrown.ClearOptions();
            currentRefreshRate = Screen.currentResolution.refreshRate;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].refreshRate == currentRefreshRate)
                {
                    filteredResolutions.Add(resolutions[i]);
                }
            }

            List<string> options = new List<string>();

            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
                options.Add(resolutionOption);

                if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdrown.AddOptions(options);
            resolutionDropdrown.value = currentResolutionIndex;
            resolutionDropdrown.RefreshShownValue();
        }
    }


    public void SetQuality(int qualityIndex)
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        AudioManager.Instance.PlaySFX("MouseKlick");

        Screen.fullScreen = isFullscreen;

        /*if (isFullscreen) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else Screen.fullScreenMode = FullScreenMode.Windowed;*/


    }
    public void SetResolution(int resolutionIndex)
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
        AudioManager.Instance.musicVolume = _musicSlider.value;
    }
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
        AudioManager.Instance.sfxVolume = _sfxSlider.value;
    }
    public void SetSlider()
    {
        _musicSlider.value = _musicVolume;
        _sfxSlider.value = _sfxVolume;
    }
    #endregion

    public void SetBoostButtonTest()
    {
        Action.action.Disable(); // critical before rebind!!
        
        // non-persistent interactive override
        rebindOperation = Action.action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.2f)
                .Start()
                .OnCancel(op =>
                {
                    //CleanUp();
                    Debug.Log("cancel");
                    //RebindOverlay.SetActive(false);
                    //KeybindMenu.SetActive(true);
                    Action.action.Enable();
                    rebindOperation.Dispose();
                })
                .OnComplete(op =>
                {
                    Action.action.Enable();
                    //SetPref(prefs, op.action.GetBindingDisplayString(1));
                    //textmesh.text = op.action.GetBindingDisplayString(1).ToUpper();
                    //StoreControlOverrides();
                    Debug.Log("saved1");
                    //RebindOverlay.SetActive(false);
                    //KeybindMenu.SetActive(true);
                    //CleanUp();
                    rebindOperation.Dispose();
                });
        // persistent non-interactive override (not sure where this is stored)
        AllTheActions.FindAction("Select").ChangeBinding(0).WithPath("<Gamepad>/buttonWest");

        // non-persistent non-interactive override (sets override path not visible in UI)
        // AllTheActions.FindAction("Select")
        //    .ApplyBindingOverride(new InputBinding
        //{
        //    overridePath = "<Gamepad>/buttonWest"
        //});
    }
}
