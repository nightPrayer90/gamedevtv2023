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

        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        if (fullScreenBtn != null)
        {
            fullScreenBtn.isOn = Screen.fullScreen;
        }

        if (resolutionDropdrown != null)
        {
            resolutions = Screen.resolutions;
            resolutionDropdrown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdrown.AddOptions(options);
            resolutionDropdrown.value = currentResolutionIndex;
            resolutionDropdrown.RefreshShownValue();
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
    public void Options()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("OptionScene");
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
    /* CREDIT SCENE---------------------------------------------------------------- */
    /* **************************************************************************** */
    public void BacktoMainMenueCredit()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("MenueScene");
    }




    /* **************************************************************************** */
    /* IN GAME--------------------------------------------------------------------- */
    /* **************************************************************************** */
    public void BacktoMainMenue()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("MenueScene");
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




    /* **************************************************************************** */
    /* VICTORY--------------------------------------------------------------------- */
    /* **************************************************************************** */
    public void BacktoMainMenueVictory()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        SceneManager.LoadScene("OutroScene");
        AudioManager.Instance.PlayMusic("MenuMusic");
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
