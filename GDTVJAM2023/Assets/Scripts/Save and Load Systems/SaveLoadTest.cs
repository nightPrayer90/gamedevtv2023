using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SaveLoadTest : MonoBehaviour
{
    [Header("All Panels")]
    public List<Sprite> sprShipPanels;
    public bool canChoose = true;

    [Header("Panel 01")]
    public TMP_InputField inputField01;
    public TextMeshProUGUI playerName01Text;
    public Button deleteButton01;
    public Image shipPane01;

    [Header("Panel 02")]
    public TMP_InputField inputField02;
    public TextMeshProUGUI playerName02Text;
    public Button deleteButton02;
    public Image shipPane02;

    [Header("Panel 03")]
    public TMP_InputField inputField03;
    public TextMeshProUGUI playerName03Text;
    public Button deleteButton03;
    public Image shipPane03;

    private PlayerStats playerStats01 = new PlayerStats();
    private PlayerStats playerStats02 = new PlayerStats(); 
    private PlayerStats playerStats03 = new PlayerStats(); 
    private IDataService DataService = new JsonDataService();
    private bool encriptionEnabled = true;
    private bool isLoad1 = false;
    private bool isLoad2 = false;
    private bool isLoad3 = false;

    private int index;

    [Header("Player Data")]
    public PlayerData playerData;


    /* **************************************************************************** */
    /* SAVE AN LOAD SYSTEM--------------------------------------------------------- */
    /* **************************************************************************** */

    // *** if the scene starts - load all existing profiles ***
    private void Awake()
    {
        LoadAllPlayerProfiles();
    }


    // *** serialize profile Panels - load all existing profiles ***
    public void LoadAllPlayerProfiles()
    {
        // Load profil one
        try
        {
            playerStats01 = DataService.LoadData<PlayerStats>("/player-stats01.json", encriptionEnabled);

            inputField01.gameObject.SetActive(false);
            playerName01Text.gameObject.SetActive(true);
            deleteButton01.gameObject.SetActive(true);
            playerName01Text.text = playerStats01.playerName.ToString();
            shipPane01.gameObject.SetActive(true);
            shipPane01.sprite = sprShipPanels[playerStats01.shipPanelIndex];
            isLoad1 = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Could not read file 01!");
            playerName01Text.gameObject.SetActive(false);
            deleteButton01.gameObject.SetActive(false);
            shipPane01.gameObject.SetActive(false);
            inputField01.text = "";
            isLoad1 = false;
        }

        // Load profil two
        try
        {
            playerStats02 = DataService.LoadData<PlayerStats>("/player-stats02.json", encriptionEnabled);

            inputField02.gameObject.SetActive(false);
            playerName02Text.gameObject.SetActive(true);
            deleteButton02.gameObject.SetActive(true);
            playerName02Text.text = playerStats02.playerName.ToString();
            shipPane02.gameObject.SetActive(true);
            shipPane02.sprite = sprShipPanels[playerStats02.shipPanelIndex];
            isLoad2 = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Could not read file 02!");
            playerName02Text.gameObject.SetActive(false);
            deleteButton02.gameObject.SetActive(false);
            shipPane02.gameObject.SetActive(false);
            inputField02.text = "";
            isLoad2 = false;
        }

        // Load profil three
        try
        {
            playerStats03 = DataService.LoadData<PlayerStats>("/player-stats03.json", encriptionEnabled);

            inputField03.gameObject.SetActive(false);
            playerName03Text.gameObject.SetActive(true);
            deleteButton03.gameObject.SetActive(true);
            playerName03Text.text = playerStats03.playerName.ToString();
            shipPane03.gameObject.SetActive(true);
            shipPane03.sprite = sprShipPanels[playerStats03.shipPanelIndex];
            isLoad3 = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Could not read file 03!");
            playerName03Text.gameObject.SetActive(false);
            deleteButton03.gameObject.SetActive(false);
            shipPane03.gameObject.SetActive(false);
            inputField03.text = "";
            isLoad3 = false;
        }
    }

   
    // *** clear Player profile 01 ***
    public void ClearData01()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        string path = Application.persistentDataPath + "/player-stats01.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            inputField01.gameObject.SetActive(true);
            playerName01Text.gameObject.SetActive(false);
            deleteButton01.gameObject.SetActive(false);
            shipPane01.gameObject.SetActive(false);
            inputField01.text = "";
            isLoad1 = false;
        }
    }

    // *** clear Player profile 02 ***
    public void ClearData02()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        string path = Application.persistentDataPath + "/player-stats02.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            inputField02.gameObject.SetActive(true);
            playerName02Text.gameObject.SetActive(false);
            deleteButton02.gameObject.SetActive(false);
            shipPane02.gameObject.SetActive(false);
            inputField02.text = "";
            isLoad2 = false;
        }
    }

    // *** clear Player profile 03 ***
    public void ClearData03()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        string path = Application.persistentDataPath + "/player-stats03.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            inputField03.gameObject.SetActive(true);
            playerName03Text.gameObject.SetActive(false);
            deleteButton03.gameObject.SetActive(false);
            shipPane03.gameObject.SetActive(false);
            inputField03.text = "";
            isLoad3 = false;
        }
    }


    // *** function create a new profile and/or fills die PlayerData Object and goes to the next scene ***
    // *** function is triggert by ProfilePanel class - player click at one panel ***
    public void SetIndex(int index_)
    {
        index = index_;
        int shipIndex = UnityEngine.Random.Range(0, 3);
        bool noSound = false;
        float delayTime = 0.3f;

        // only the current Profil is not exist - create a new Profile
        switch (index_)
        {
            case 0:
                if (isLoad1 == false && inputField01.text != "")
                {
                    // Save new Profile
                    playerStats01 = new PlayerStats();
                    playerStats01.playerName = inputField01.text;
                    playerStats01.shipPanelIndex = shipIndex;
                    playerStats01.savePath = "/player-stats01.json";
                    DataService.SaveData(playerStats01.savePath, playerStats01, encriptionEnabled);

                    shipPane01.gameObject.SetActive(true);
                    shipPane01.sprite = sprShipPanels[shipIndex];


                    playerName01Text.text = inputField01.text;
                    playerName01Text.gameObject.SetActive(true);
                    inputField01.gameObject.SetActive(false);

                    deleteButton01.gameObject.SetActive(false);
                    deleteButton02.gameObject.SetActive(false);
                    deleteButton03.gameObject.SetActive(false);
                    delayTime = 1f;
                }

                if (inputField01.text == "" && isLoad1 == false)
                {
                    // inputfield has no text
                    noSound = true;
                    canChoose = true;
                    inputField01.transform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1);
                }
                break;

            case 1:
                if (isLoad2 == false && inputField02.text != "")
                {
                    // Save new Profile
                    playerStats02 = new PlayerStats();
                    playerStats02.playerName = inputField02.text;
                    playerStats02.shipPanelIndex = shipIndex;
                    playerStats02.savePath = "/player-stats02.json";
                    DataService.SaveData(playerStats02.savePath, playerStats02, encriptionEnabled);

                    shipPane02.gameObject.SetActive(true);
                    shipPane02.sprite = sprShipPanels[shipIndex];

                    playerName02Text.text = inputField02.text;
                    playerName02Text.gameObject.SetActive(true);
                    inputField02.gameObject.SetActive(false);

                    deleteButton01.gameObject.SetActive(false);
                    deleteButton02.gameObject.SetActive(false);
                    deleteButton03.gameObject.SetActive(false);
                    delayTime = 1f;
                }

                if (inputField02.text == "" && isLoad2 == false)
                {
                    // inputfield has no text
                    noSound = true;
                    canChoose = true;
                    inputField02.transform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1);
                }
                break;

            case 2:
                if (isLoad3 == false && inputField03.text != "")
                {
                    // Save new Profile
                    playerStats03 = new PlayerStats();
                    playerStats03.playerName = inputField03.text;
                    playerStats03.shipPanelIndex = shipIndex;
                    playerStats03.savePath = "/player-stats03.json";
                    DataService.SaveData(playerStats03.savePath, playerStats03, encriptionEnabled);

                    shipPane03.gameObject.SetActive(true);
                    shipPane03.sprite = sprShipPanels[shipIndex];

                    playerName03Text.text = inputField03.text;
                    playerName03Text.gameObject.SetActive(true);
                    inputField03.gameObject.SetActive(false);

                    deleteButton01.gameObject.SetActive(false);
                    deleteButton02.gameObject.SetActive(false);
                    deleteButton03.gameObject.SetActive(false);
                    delayTime = 1f;
                }

                if (inputField03.text == "" && isLoad3 == false)
                {
                    // inputfield has no text
                    noSound = true;
                    canChoose = true;
                    inputField03.transform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1);
                }
                break;
        }


        // write Profile Data from the PlayerStats to the PlayerData Object
        if (noSound == true)
        {
            AudioManager.Instance.PlaySFX("MouseNo");
        }
        else
        {
            if (delayTime == 0.3f)
                AudioManager.Instance.PlaySFX("MouseKlick");
            else
                AudioManager.Instance.PlaySFX("WindowOpen");

            switch (index_)
            {
                case 0:
                    WriteOnPlayerData(playerStats01);
                    shipPane01.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), delayTime, 5, 1).OnComplete(() => { SceneManager.LoadScene("MenueScene"); });
                    break;

                case 1:
                    WriteOnPlayerData(playerStats02);
                    shipPane02.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), delayTime, 5, 1).OnComplete(() => { SceneManager.LoadScene("MenueScene"); });
                    break;

                case 2:
                    WriteOnPlayerData(playerStats03);
                    shipPane03.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), delayTime, 5, 1).OnComplete(() => { SceneManager.LoadScene("MenueScene"); });
                    break;
            }
        }
    }


    // *** write all loaded values in the PlayerData object *** 
    public void WriteOnPlayerData(PlayerStats playerStats)
    {
        // Player Profile
        playerData.playerName = playerStats.playerName;
        playerData.savePath = playerStats.savePath;
        playerData.playerShipIcon = playerStats.shipPanelIndex;

        // Hangar
        playerData.playerShip = playerStats.playerShip;
        playerData.playerShipCount = playerStats.playerShipCount;
        playerData.expBullet = playerStats.expBullet;
        playerData.expRocket = playerStats.expRocket;
        playerData.expLaser = playerStats.expLaser;
        playerData.globalUpgradeCountBullet = playerStats.globalUpgradeCountBullet;
        playerData.globalUpgradeCountRocket = playerStats.globalUpgradeCountRocket;
        playerData.globalUpgradeCountLaser = playerStats.globalUpgradeCountLaser;

        playerData.bulletResearchedSkills.Clear();
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_1);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_2);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_3);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_4);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_5);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_6);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_7);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_8);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_9);
        playerData.bulletResearchedSkills.Add(playerStats.bulletRS_10);

        playerData.rocketResearchedSkills.Clear();
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_1);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_2);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_3);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_4);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_5);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_6);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_7);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_8);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_9);
        playerData.rocketResearchedSkills.Add(playerStats.rocketRS_10);

        playerData.laserResearchedSkills.Clear();
        playerData.laserResearchedSkills.Add(playerStats.laserRS_1);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_2);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_3);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_4);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_5);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_6);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_7);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_8);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_9);
        playerData.laserResearchedSkills.Add(playerStats.laserRS_10);
    }

}
