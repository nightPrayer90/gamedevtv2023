
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    public string gameScene = "GameScene";
    public Slider _musicSlider, _sfxSlider;

    private float _musicVolume;
    private float _sfxVolume;


    public void Start()
    {
        if (_musicSlider != null)
        {
            _musicVolume = AudioManager.Instance.musicVolume;
            _sfxVolume = AudioManager.Instance.sfxVolume;

            AudioManager.Instance.MusicVolume(_musicVolume);
            AudioManager.Instance.SFXVolume(_sfxVolume);

            SetSlider();
        }
    }


    //Main Menu
    public void GameStart()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene(gameScene);
    }
    public void Credits()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("CreditScene");
    }

    public void GameQuit()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        Application.Quit();
    }

    public void Encarta()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("Encarta");
    }

    //Cedit Scene
    public void BacktoMainMenueCredit()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("MenueScene");
    }
    public void BacktoMainMenue()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("MenueScene");
        AudioManager.Instance.PlayMusic("MenuMusic");
    }

    public void BacktoMainMenueVictory()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("OutroScene");
        AudioManager.Instance.PlayMusic("MenuMusic");
    }

    public void LevelRestart()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LevelPauseStop()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        GameObject.Find("Game Manager").GetComponent<GameManager>().PauseMenue();
       
    }

    void OnMouseEnter()
    {
        AudioManager.Instance.PlaySFX("MouseHover");
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
        AudioManager.Instance.musicVolume = _musicSlider.value ;
        
    }
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
        AudioManager.Instance.sfxVolume = _sfxSlider.value;
    }

    public void SetSlider()
    {
        _musicSlider.value = _musicVolume;
        _sfxSlider.value = _sfxVolume;
    }
}
