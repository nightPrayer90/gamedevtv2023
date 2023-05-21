
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    public string gameScene = "GameScene";

    //Main Menu
    public void GameStart()
    {
        SceneManager.LoadScene(gameScene);
    }
    public void Credits()
    {
        SceneManager.LoadScene("CreditScene");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    //Cedit Scene
    public void BacktoMainMenue()
    {
        SceneManager.LoadScene("MenueScene");
    }

    public void LevelRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetTimesclae()
    {
        Time.timeScale = 1;
    }
}
