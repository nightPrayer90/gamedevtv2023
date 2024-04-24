using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using TMPro;

public class StartGameController : MonoBehaviour
{
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

    private void Start()
    {
        loadUITransform.localPosition = new Vector3(0, 900f, 0);

        loadUICanvasGroup.alpha = 0;
        loadUICanvasGroup.blocksRaycasts = false;

        LoadPlayerData();
    }

    private void Update()
    {
        // Fade out
        if (Input.GetKeyDown(KeyCode.Escape) && loadUIFlag == true)
        {
            FadeLoadUIOut();
        }
    }

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
            path = $"playerData{i+1}.json";
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

        playerStats[index-1] = newplayerStats;

        UpdateLoadPanel(index-1);
    }
}
