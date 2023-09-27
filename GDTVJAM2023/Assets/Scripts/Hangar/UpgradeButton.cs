using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public PlayerData playerData;
    public int upgradePanelIndex; // 0=bullet; 1=rocket; 2=laser
    public int upgradeButtonIndex;
    public int Colorindex;
    private bool isComplete = false;
    public Image btnImage;
    public HangarManager gameManager;

    private void OnEnable()
    {
        if (isComplete == true)
        {
            btnImage.color = gameManager.classColors[Colorindex];
        }
        else
        {
            btnImage.color = Color.white;
        }
    }

    public void UpgradeAbility()
    {
        if (isComplete == false)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");
            isComplete = true;
            btnImage.color = gameManager.classColors[Colorindex];
        }
        else
        {
            AudioManager.Instance.PlaySFX("MouseNo");
        }
    }
}
