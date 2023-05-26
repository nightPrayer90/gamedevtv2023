
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    public string gameScene = "GameScene";
    public Slider _musicSlider, _sfxSlider;

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

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }
    public void ToggleSfx()
    {
        AudioManager.Instance.ToggleSFX();
    }
    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }
}
