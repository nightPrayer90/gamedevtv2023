using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UpgradeShipPanelController : MonoBehaviour
{
    private Vector3 localPositon;

    [HideInInspector] public List<Color> classColors;
    //public List<Image> classPanels = new List<Image>();//

    //public List<Image> selectedUpgradePanelList = new List<Image>();


    //Objects
    private GameManager gameManager;
    //private UpgradeChooseList upgradeChooseList;
    //private PlayerController playerController;
    //private PlayerWeaponController playerWeaponController;

    public Image ShipPanel;
    public List<Sprite> shipImages = new List<Sprite>();

    private void Awake()
    {
        // save the first local position
        localPositon = gameObject.transform.position;

        // set Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        //classColors = new List<Color>(gameManager.globalClassColor);
    }
    private void OnEnable()
    {
        // set Objects
        //playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        //playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();

        // Update content
        //UpdateValuePanel();

        // FadeIn Tween
        transform.position = localPositon;
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f, 8, 1).SetUpdate(true);

        // set Ship Panel Image
        switch (gameManager.startShip)
        {
            case GameManager.StartShip.bullet:
                ShipPanel.sprite = shipImages[0];
                break;
            case GameManager.StartShip.laser:
                ShipPanel.sprite = shipImages[1];
                break;
            case GameManager.StartShip.rocket:
                ShipPanel.sprite = shipImages[2];
                break;


        }
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

        Color defaultColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        // update class colors
        /*if (upgradeChooseList.mcBulletLvl > 0) { classPanels[0].color = classColors[0]; } else { classPanels[0].color = defaultColor; }
        if (upgradeChooseList.mcExplosionLvl > 0) { classPanels[1].color = classColors[1]; } else { classPanels[1].color = defaultColor; }
        if (upgradeChooseList.mcLaserLvl > 0) { classPanels[2].color = classColors[2]; } else { classPanels[2].color = defaultColor; }
        if (upgradeChooseList.mcSupportLvl > 0) { classPanels[3].color = classColors[3]; } else { classPanels[3].color = defaultColor; }

        if (upgradeChooseList.scSwarmLvl > 0) { classPanels[4].color = classColors[4]; } else { classPanels[4].color = defaultColor; }
        if (upgradeChooseList.scDefenceLvl > 0) { classPanels[5].color = classColors[5]; } else { classPanels[5].color = defaultColor; }
        if (upgradeChooseList.scTargetingLvl > 0) { classPanels[6].color = classColors[6]; } else { classPanels[6].color = defaultColor; }
        if (upgradeChooseList.scDirectionLvl > 0) { classPanels[7].color = classColors[7]; } else { classPanels[7].color = defaultColor; }*/
    }

}
