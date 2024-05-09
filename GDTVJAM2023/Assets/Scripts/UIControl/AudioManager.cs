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

        if (s == null)
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
    // ToDo: Maybe move in another object

    #region Load an Save stuff
    /// <summary>
    /// this function moves all data from the playerData to the playerStats and saves this data
    /// </summary>
    public void SavePlayerData()
    {
        PlayerStats playerStats = new(playerData);

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

        playerData.Initialize(path_, moduleList.moduls.Count, uLPrefab.upgradeList.Count);
    }

    public void LoadPlayerData(string path)
    {
        PlayerStats playerStats = DataService.LoadData<PlayerStats>(path, false); //"playerData.json"
        playerData.LoadStats(playerStats);
    }

    // --------------------------------------------
    // Transition Stuff ---------------------------
    public void SceneTransition(string sceneName, int panelIndex = 0)
    {
        transition.Transition(sceneName, panelIndex);
    }
    #endregion
}
