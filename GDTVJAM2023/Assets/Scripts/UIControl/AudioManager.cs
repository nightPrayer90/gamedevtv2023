using UnityEngine;
using System;
using System.IO;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    public float musicVolume, sfxVolume;
    public PlayerData playerData;
    private IDataService DataService = new JsonDataService();
    private bool encriptionEnabled = false;
    public ModuleList moduleList;
    public UpgradeList uLPrefab;
    public SimpleSceneTransition transition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayMusic("MenuMusic");
    }


    // How to use:
    // AudioManager.Instance.PlaySFX("name");
    // AudioManager.Instance.PlayMusic("name");
    // AudioManager.Instance.musicSource.Stop(); - Stop Music

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s==null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.Stop();
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
        public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    // --------------------------------------------------------------------------------------------
    // save and load stuff
    // ToDo: Maybe move in another object
    // this function move all data in form the playerData to the playerStats and saves this data
    #region Load an Save stuff
    public void SavePlayerData()
    {
        PlayerStats playerStats = new();

        //Debug.Log("SaveDataStart---- " + playerData.name);
        // PlayerProfil
        playerStats.playerName = playerData.playerName;
        playerStats.savePath = playerData.savePath;
        playerStats.shipPanelIndex = playerData.playerShipIcon;

        // Hangar and Shop
        foreach (ModuleData module in playerData.moduleData)
        {
            playerStats.moduleData.Add(module);
        }

        foreach (int i in playerData.moduleCounts)
        {
            playerStats.moduleCounts.Add(i);
        }

        playerStats.credits = playerData.credits;
        playerStats.bossLevel = playerData.bossLevel;
        playerStats.shopLevelVisited = playerData.shopLevelVisited;

        // Skillboard
        foreach (bool i in playerData.skillsSpotted)
        {
            playerStats.skillsSpotted.Add(i);
        }
        
        DataService.SaveData(playerData.savePath, playerStats, encriptionEnabled);
        //Debug.Log("SaveDataEnd---- " + playerStats.playerName);
    }

    public void SetPlayerDataToDefault(String path_)
    {
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + path_;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
       
        playerData.playerName = "Player";
        playerData.savePath = path_; //"playerData.json";
        playerData.playerShipIcon = 0;

        // Hangar and Shop
        playerData.credits = 0;
        playerData.bossLevel = 0;
        playerData.moduleCounts = new();
        playerData.moduleData = new();
        playerData.skillsSpotted = new();
        playerData.shopLevelVisited = 0;

        for (int i = 0; i< moduleList.moduls.Count; i++)
        {
            playerData.moduleCounts.Add(0);
        }

        // Skill Board
        for (int i = 0; i < uLPrefab.upgradeList.Count; i++)
        {
            playerData.skillsSpotted.Add(false);
        }
    }

    public void LoadPlayerData(string path)
    {
        PlayerStats playerStats = new();

        playerStats = DataService.LoadData<PlayerStats>(path, false); //"playerData.json"

        // PlayerStuff
        playerData.playerName = playerStats.playerName;
        playerData.savePath = playerStats.savePath;
        playerData.playerShipIcon = playerStats.shipPanelIndex;

        // Hangar and Shop
        playerData.credits = playerStats.credits;
        playerData.bossLevel = playerStats.bossLevel;
        playerData.shopLevelVisited = playerStats.shopLevelVisited;


        playerData.moduleData.Clear();
        foreach (ModuleData i in playerStats.moduleData)
        {
            playerData.moduleData.Add(i);
        }

        playerData.moduleCounts.Clear();
        foreach (int i in playerStats.moduleCounts)
        {
            playerData.moduleCounts.Add(i);
        }

        // Skill Board
        playerData.skillsSpotted.Clear();
        foreach (bool b in playerStats.skillsSpotted)
        {
            playerData.skillsSpotted.Add(b);
        }
    }

    // --------------------------------------------
    // Transition Stuff ---------------------------
   public void SceneTransition(string sceneName, int panelIndex = 0)
    {
        transition.Transition(sceneName, panelIndex);
    }
    #endregion
}
