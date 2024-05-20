using DG.Tweening;
using UnityEngine;

public class SkillBordController : MonoBehaviour
{
    public bool debugMode = false;
    public UpgradeList ulPrefab;
    public ClassColor ccPrefab;
    public PlayerData playerData;

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToHangar();
        }*/
    }

    public void GoBackToHangar()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        AudioManager.Instance.SceneTransition("HangarScene");
    }
}
