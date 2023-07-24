using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using TMPro;

public class SaveLoadTest : MonoBehaviour
{
    public TextMeshProUGUI sourceDataText;
    public TMP_InputField inputField;
    public TextMeshProUGUI saveTimeText;
    public TextMeshProUGUI loadTimeText;

    private PlayerStats playerStats = new PlayerStats();  //- klasse muss ich noch herausfinden
    private IDataService DataService = new JsonDataService();
    private bool encriptionEnabled = true;
    private long saveTime;
    private long loadTime;

    public void SerializeJson()
    {
        long startTime = DateTime.Now.Ticks;
        if (DataService.SaveData("/player-stats.json", playerStats, encriptionEnabled))
        {
            saveTime = DateTime.Now.Ticks - startTime;
            saveTimeText.SetText($"Save Time: {(saveTime / 1000f):N4}ms");
            
            startTime = DateTime.Now.Ticks;
            try
            {
                PlayerStats data = DataService.LoadData<PlayerStats>("/player-stats.json", encriptionEnabled);
                loadTime = DateTime.Now.Ticks - startTime;
                inputField.text = "Loaded from file:\r\n" + JsonConvert.SerializeObject(data, Formatting.Indented);
                loadTimeText.SetText($"Load Time: {(loadTime / 1000f):N4}ms");
            }
            catch(Exception e)
            {
                Debug.LogError($"Could not read file! Show something on the UI here!");
                inputField.text = "<color=#ff0000>Error reading save file!</color>";
            }
        }
        else
        {
            Debug.LogError("Could not save file! Sow something on the UI about it!");
            inputField.text = "<color=#ff0000>Error saving data! </color>";
        }
    }

    // LOAD: -------
    private void Awake()
    {
        sourceDataText.SetText(JsonConvert.SerializeObject(playerStats, Formatting.Indented));
    }

    public void ClearData()
    {
        string path = Application.persistentDataPath + "/player-stats.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            inputField.text = "Loaded data goes here";
        }
    }
}
