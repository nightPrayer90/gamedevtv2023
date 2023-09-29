using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveAndStart : MonoBehaviour
{
    public string gameScene;
    public PlayerData playerData;
    private AsyncOperation asyncLoad;

    private void Start()
    {
        // load GameScene
        StartCoroutine(LoadYourAsyncScene());

        // load playerData (only for Editorusing)
        if (playerData.savePath == "") 
        {
            //LoadPlayerData();
        }
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.

        asyncLoad = SceneManager.LoadSceneAsync(gameScene);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void GameStart()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SavePlayerData();
        asyncLoad.allowSceneActivation = true;
    }

    // Save Player Data while the Game Starts
    public void SavePlayerData()
    {
        AudioManager.Instance.SavePlayerData();
    }

    // only for Editor Runtime
    private void LoadPlayerData()
    {
        playerData.savePath = AudioManager.Instance.playerStats.savePath;
        Debug.Log(AudioManager.Instance.playerStats.playerName);
        playerData.playerName = AudioManager.Instance.playerStats.playerName;
    }
}
