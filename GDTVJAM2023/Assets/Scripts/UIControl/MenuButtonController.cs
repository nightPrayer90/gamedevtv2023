using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuButtonController : MonoBehaviour
{
    public string gameScene = "GameScene";
    public Slider _musicSlider, _sfxSlider;

    private float _musicVolume;
    private float _sfxVolume;

    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdrown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullScreenBtn;

    

    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
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
    void OnMouseEnter()
    {
        AudioManager.Instance.PlaySFX("MouseHover");
    }




    /* **************************************************************************** */
    /* MAIN MENU------------------------------------------------------------------- */
    /* **************************************************************************** */
    public void GameStart()
    {
        //AudioManager.Instance.SetPlayerDataToDefault();
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene("IntroScene");
        AudioManager.Instance.SceneTransition("IntroScene");
    }
    public void GameLoad()
    {
        //AudioManager.Instance.LoadPlayerData();
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene(gameScene);
        AudioManager.Instance.SceneTransition(gameScene);
    }
    public void Credits()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        AudioManager.Instance.SceneTransition("CreditScene",1);
        //SceneManager.LoadScene("CreditScene");
    }
    public void GameQuit()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        Application.Quit();
    }
    public void Encarta()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene("Encarta");
        AudioManager.Instance.SceneTransition("Encarta",1);
    }
    public void Options()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene("OptionScene");
        AudioManager.Instance.SceneTransition("OptionScene",1);
    }




    /* **************************************************************************** */
    /* OPTION---------------------------------------------------------------------- */
    /* **************************************************************************** */
    public void SetQuality(int qualityIndex)
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution (int resolutionIndex)
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
        AudioManager.Instance.musicVolume = _musicSlider.value;
    }
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
        AudioManager.Instance.sfxVolume = _sfxSlider.value;
    }



    /* **************************************************************************** */
    /* IN GAME--------------------------------------------------------------------- */
    /* **************************************************************************** */
    public void BacktoMainMenue()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene("HangarScene");
        AudioManager.Instance.PlayMusic("MenueMusic");
        AudioManager.Instance.SceneTransition("MenueScene");
    }
    public void LevelRestart()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AudioManager.Instance.SceneTransition(SceneManager.GetActiveScene().name);
    }

    public void BacktoHangar()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene("HangarScene");
        AudioManager.Instance.PlayMusic("MenuMusic");
        AudioManager.Instance.SceneTransition("HangarScene");
    }



    /* **************************************************************************** */
    /* VICTORY--------------------------------------------------------------------- */
    /* **************************************************************************** */
    public void BacktoMainMenueVictory()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        AudioManager.Instance.PlayMusic("MenuMusic");
        AudioManager.Instance.SceneTransition("OutroScene");
    }



    /* **************************************************************************** */
    /* HELP FUNCTIONS----*--------------------------------------------------------- */
    /* **************************************************************************** */
    public void SetSlider()
    {
        _musicSlider.value = _musicVolume;
        _sfxSlider.value = _sfxVolume;
    }




    /* **************************************************************************** */
    /* NOT IN USE------------------------------------------------------------------ */
    /* **************************************************************************** */
    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }
    public void ToggleSfx()
    {
        AudioManager.Instance.ToggleSFX();
    }
}
