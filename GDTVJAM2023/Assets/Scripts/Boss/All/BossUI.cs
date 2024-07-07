using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [Header("Boss UI")]
    public GameObject bossHud;
    public CanvasGroup bossHudCg;
    public Slider bossHealthSlider;
    public Image bossHealthForeground;
    public GameManager gameManager;


    public void InitHealthBar(float bosshealth_, Sprite forgroundSprite)
    {
        // healthbar control
        bossHudCg.alpha = 0;
        bossHealthForeground.sprite = forgroundSprite;
        bossHealthForeground.color = Color.red;
        bossHealthSlider.maxValue = bosshealth_;
    }

    public void OpenBossUI()
    {
        bossHud.SetActive(true);
        bossHudCg.DOFade(1f, 0.2f);
        bossHealthSlider.value = 0;

        bossHealthSlider.DOValue(bossHealthSlider.maxValue, 5.2f).OnComplete(() =>
        {
            bossHealthSlider.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1.5f, 10, 1f);
        });
    }

    public void SetForgroundColor(Color color)
    {
        bossHealthForeground.color = color;
    }

    public void UpdateSliderValue(float enemyHealth_)
    {
        bossHealthSlider.value = enemyHealth_;
    }

    public void FadeOut()
    {
        bossHudCg.DOFade(0f, 0.5f).OnComplete(() => { bossHud.SetActive(false); });
    }
}
