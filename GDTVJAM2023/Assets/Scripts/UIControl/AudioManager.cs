using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;
using DG.Tweening.Plugins.Core.PathCore;


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

    // ToDo: Maybe move in another object
    // this function move all data in form the playerData to the playerStats and saves this data
    public void SavePlayerData()
    {
        PlayerStats playerStats = new();

        //Debug.Log("SaveDataStart---- " + playerData.name);
        // PlayerProfil
        playerStats.playerName = playerData.playerName;
        playerStats.savePath = playerData.savePath;
        playerStats.shipPanelIndex = playerData.playerShipIcon;

        // Hangar and Shop
        foreach (int i in playerData.moduleCounts)
        {
            playerStats.moduleCounts.Add(i);
        }

        playerStats.credits = playerData.credits;
        playerStats.bossLevel = playerData.bossLevel;


        DataService.SaveData(playerData.savePath, playerStats, encriptionEnabled);
        //Debug.Log("SaveDataEnd---- " + playerStats.playerName);
    }

    public void SetPlayerDataToDefault()
    {
        string path = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + playerData.savePath;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        //TODO save Modules in SaveData
        path = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "modules.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        playerData.playerName = "Player";
        playerData.savePath = "playerData.json";
        playerData.playerShipIcon = 0;

        // Hangar and Shop
        playerData.credits = 0;
        playerData.bossLevel = 0;
        playerData.moduleCounts = new();
        // parts for first ship
        playerData.moduleCounts.Add(1);
        playerData.moduleCounts.Add(1);
        playerData.moduleCounts.Add(1);
        playerData.moduleCounts.Add(1);
        playerData.moduleCounts.Add(2);
        for (int i = 5; i< moduleList.moduls.Count; i++)
        {
            playerData.moduleCounts.Add(0);
        }
    }

    public void LoadPlayerData()
    {
        PlayerStats playerStats = new();

        playerStats = DataService.LoadData<PlayerStats>("playerData.json", false);

        // PlayerStuff
        playerData.playerName = playerStats.playerName;
        playerData.savePath = playerStats.savePath;
        playerData.playerShipIcon = playerStats.shipPanelIndex;

        // Hangar and Shop
        playerData.credits = playerStats.credits;
        playerData.bossLevel = playerStats.bossLevel;

        playerData.moduleCounts.Clear();
        foreach (int i in playerStats.moduleCounts)
        {
            playerData.moduleCounts.Add(i);
        }
    }
}
