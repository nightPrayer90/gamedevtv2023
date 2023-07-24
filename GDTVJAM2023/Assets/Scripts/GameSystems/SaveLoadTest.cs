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
    [Header("All Panel")]
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

    public void SetIndex(int index_)
    {
        Debug.Log(index);
        index = index_;
        int shipIndex = UnityEngine.Random.Range(0,3);
        bool noSound = false;
        float delayTime = 0.3f;

        switch(index_)
        {
            case 0:
                if (isLoad1 == false && inputField01.text != "")
                {  
                    // Save new Profile
                    playerStats01.playerName = inputField01.text;
                    playerStats01.shipPanelIndex = shipIndex;
                    DataService.SaveData("/player-stats01.json", playerStats01, encriptionEnabled);

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
                    playerStats02.playerName = inputField02.text;
                    playerStats02.shipPanelIndex = shipIndex;
                    DataService.SaveData("/player-stats02.json", playerStats02, encriptionEnabled);

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
                    playerStats03.playerName = inputField03.text;
                    playerStats03.shipPanelIndex = shipIndex;
                    DataService.SaveData("/player-stats03.json", playerStats03, encriptionEnabled);

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
                    shipPane01.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), delayTime, 5, 1).OnComplete(() => { SceneManager.LoadScene("MenueScene"); });
                    break;

                case 1:
                    shipPane02.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), delayTime, 5, 1).OnComplete(() => { SceneManager.LoadScene("MenueScene"); });
                    break;

                case 2:
                    shipPane03.transform.DOPunchScale(new Vector3(0.35f, 0.35f, 0.35f), delayTime, 5, 1).OnComplete(() => { SceneManager.LoadScene("MenueScene"); });
                    break;
            }
        }
    }


    /* **************************************************************************** */
    /* SAVE AN LOAD SYSTEM--------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        SerializePlayer01();
        SerializePlayer02();
        SerializePlayer03();
    }

   

    public void SerializePlayer01()
    {
        try
        {
            PlayerStats data = DataService.LoadData<PlayerStats>("/player-stats01.json", encriptionEnabled);

            inputField01.gameObject.SetActive(false);
            playerName01Text.gameObject.SetActive(true);
            deleteButton01.gameObject.SetActive(true);
            playerName01Text.text = data.playerName.ToString();
            shipPane01.gameObject.SetActive(true);
            shipPane01.sprite = sprShipPanels[data.shipPanelIndex];
            isLoad1 = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Could not read file!");
            playerName01Text.gameObject.SetActive(false);
            deleteButton01.gameObject.SetActive(false);
            shipPane01.gameObject.SetActive(false);
            inputField01.text = "";
            isLoad1 = false;
        }
    }

    public void SerializePlayer02()
    {
        try
        {
            PlayerStats data = DataService.LoadData<PlayerStats>("/player-stats02.json", encriptionEnabled);

            inputField02.gameObject.SetActive(false);
            playerName02Text.gameObject.SetActive(true);
            deleteButton02.gameObject.SetActive(true);
            playerName02Text.text = data.playerName.ToString();
            shipPane02.gameObject.SetActive(true);
            shipPane02.sprite = sprShipPanels[data.shipPanelIndex];
            isLoad2 = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Could not read file!");
            playerName02Text.gameObject.SetActive(false);
            deleteButton02.gameObject.SetActive(false);
            shipPane02.gameObject.SetActive(false);
            inputField02.text = "";
            isLoad2 = false;
        }
    }

    public void SerializePlayer03()
    {
        try
        {
            PlayerStats data = DataService.LoadData<PlayerStats>("/player-stats03.json", encriptionEnabled);

            inputField03.gameObject.SetActive(false);
            playerName03Text.gameObject.SetActive(true);
            deleteButton03.gameObject.SetActive(true);
            playerName03Text.text = data.playerName.ToString();
            shipPane03.gameObject.SetActive(true);
            shipPane03.sprite = sprShipPanels[data.shipPanelIndex];
            isLoad3 = true;
        }
        catch (Exception e)
        {
            Debug.Log($"Could not read file!");
            playerName03Text.gameObject.SetActive(false);
            deleteButton03.gameObject.SetActive(false);
            shipPane03.gameObject.SetActive(false);
            inputField03.text = "";
            isLoad3 = false;
        }
    }

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
}
