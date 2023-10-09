using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    public float musicVolume, sfxVolume;
    public PlayerData playerData;
    public PlayerStats playerStats = new PlayerStats();
    private IDataService DataService = new JsonDataService();
    private bool encriptionEnabled = true;


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
        //Debug.Log("SaveDataStart---- " + playerData.name);
        // PlayerProfil
        playerStats.playerName = playerData.playerName;
        playerStats.savePath = playerData.savePath;
        playerStats.shipPanelIndex = playerData.playerShipIcon;

        // Hangar
        playerStats.playerShip = playerData.playerShip;
        playerStats.playerShipCount = playerData.playerShipCount;
        playerStats.expBullet = playerData.expBullet;
        playerStats.expRocket = playerData.expRocket;
        playerStats.expLaser = playerData.expLaser;
        playerStats.globalUpgradeCountBullet = playerData.globalUpgradeCountBullet;
        playerStats.globalUpgradeCountRocket = playerData.globalUpgradeCountRocket;
        playerStats.globalUpgradeCountLaser = playerData.globalUpgradeCountLaser;

        playerStats.bulletRS_1 = playerData.bulletResearchedSkills[0];
        playerStats.bulletRS_2 = playerData.bulletResearchedSkills[1];
        playerStats.bulletRS_3 = playerData.bulletResearchedSkills[2];
        playerStats.bulletRS_4 = playerData.bulletResearchedSkills[3];
        playerStats.bulletRS_5 = playerData.bulletResearchedSkills[4];
        playerStats.bulletRS_6 = playerData.bulletResearchedSkills[5];
        playerStats.bulletRS_7 = playerData.bulletResearchedSkills[6];
        playerStats.bulletRS_8 = playerData.bulletResearchedSkills[7];
        playerStats.bulletRS_9 = playerData.bulletResearchedSkills[8];
        playerStats.bulletRS_10 = playerData.bulletResearchedSkills[9];

        playerStats.rocketRS_1 = playerData.rocketResearchedSkills[0];
        playerStats.rocketRS_2 = playerData.rocketResearchedSkills[1];
        playerStats.rocketRS_3 = playerData.rocketResearchedSkills[2];
        playerStats.rocketRS_4 = playerData.rocketResearchedSkills[3];
        playerStats.rocketRS_5 = playerData.rocketResearchedSkills[4];
        playerStats.rocketRS_6 = playerData.rocketResearchedSkills[5];
        playerStats.rocketRS_7 = playerData.rocketResearchedSkills[6];
        playerStats.rocketRS_8 = playerData.rocketResearchedSkills[7];
        playerStats.rocketRS_9 = playerData.rocketResearchedSkills[8];
        playerStats.rocketRS_10 = playerData.rocketResearchedSkills[9];

        playerStats.laserRS_1 = playerData.laserResearchedSkills[0];
        playerStats.laserRS_2 = playerData.laserResearchedSkills[1]; 
        playerStats.laserRS_3 = playerData.laserResearchedSkills[2]; 
        playerStats.laserRS_4 = playerData.laserResearchedSkills[3]; 
        playerStats.laserRS_5 = playerData.laserResearchedSkills[4]; 
        playerStats.laserRS_6 = playerData.laserResearchedSkills[5]; 
        playerStats.laserRS_7 = playerData.laserResearchedSkills[6]; 
        playerStats.laserRS_8 = playerData.laserResearchedSkills[7]; 
        playerStats.laserRS_9 = playerData.laserResearchedSkills[8]; 
        playerStats.laserRS_10 = playerData.laserResearchedSkills[9];

      
        DataService.SaveData(playerData.savePath, playerStats, encriptionEnabled);
        //Debug.Log("SaveDataEnd---- " + playerStats.playerName);
    }


}
