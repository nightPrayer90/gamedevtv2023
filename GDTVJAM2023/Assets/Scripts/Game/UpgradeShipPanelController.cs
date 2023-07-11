using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UpgradeShipPanelController : MonoBehaviour
{
    private Vector3 localPositon;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI pickupText;

    [HideInInspector] public List<Color> classColors;
    public List<Image> classPanels = new List<Image>();

    //public List<Image> selectedUpgradePanelList = new List<Image>();


    //Objects
    private GameManager gameManager;
    private PlayerController playerController;
    private PlayerWeaponController playerWeaponController;


    private void Awake()
    {
        // save the first local position
        localPositon = gameObject.transform.position;

        // set Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        classColors = new List<Color>(gameManager.globalClassColor);
    }
    private void OnEnable()
    {
        // set Objects
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();

        // Update content
        UpdateValuePanel();

        // FadeIn Tween
        transform.position = localPositon;
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f, 8, 1).SetUpdate(true);
    }

    // FadeOut Tween
    public void FadeOut()
    {
        transform.DOLocalMoveY(-600, 0.5f, true).SetUpdate(true).SetEase(Ease.InQuart);
    }

    // Update Values in Panel 4
    public void UpdateValuePanel()
    {
        // update main weapon valuesText
        lifeText.text = playerController.playerMaxHealth.ToString();
        damageText.text = playerController.playerBulletBaseDamage.ToString();
        rateText.text = playerController.playerFireRate.ToString();
        speedText.text = (playerController.speed / 100).ToString();
        agilityText.text = playerController.rotateSpeed.ToString();
        pickupText.text = playerController.pickupRange.ToString();

        Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        // update class colors
        if (playerWeaponController.mcBulletLvl > 0) { classPanels[0].color = classColors[0]; } else { classPanels[0].color = defaultColor; }
        if (playerWeaponController.mcExplosionLvl > 0) { classPanels[1].color = classColors[1]; } else { classPanels[1].color = defaultColor; }
        if (playerWeaponController.mcLaserLvl > 0) { classPanels[2].color = classColors[2]; } else { classPanels[2].color = defaultColor; }
        if (playerWeaponController.mcSupportLvl > 0) { classPanels[3].color = classColors[3]; } else { classPanels[3].color = defaultColor; }

        if (playerWeaponController.scSwarmLvl > 0) { classPanels[4].color = classColors[4]; } else { classPanels[4].color = defaultColor; }
        if (playerWeaponController.scDefenceLvl > 0) { classPanels[5].color = classColors[5]; } else { classPanels[5].color = defaultColor; }
        if (playerWeaponController.scTargetingLvl > 0) { classPanels[6].color = classColors[6]; } else { classPanels[6].color = defaultColor; }
        if (playerWeaponController.scBackwardsLvl > 0) { classPanels[7].color = classColors[7]; } else { classPanels[7].color = defaultColor; }
    }

}
