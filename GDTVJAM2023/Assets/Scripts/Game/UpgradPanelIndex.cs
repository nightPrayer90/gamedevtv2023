using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UpgradPanelIndex : MonoBehaviour
{
    [Header("Panel Settings")]

    public int index;
    public Image panelImage;

    [Header("Panel Values")]
    public UpgradePanelController upgradePanelController;
    public Sprite spPanelSelect;
    public Sprite spPanelDeselcet;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
    public Image iconPanel;
    public TextMeshProUGUI mainClass;
    private bool isTweening = true;
    public bool isSelected = false;
    private UpgradeChooseList upgradeChooseList;
    private int upgradeIndex = -1;

    [Header("Requirement System")]
    public Image req1;
    public TextMeshProUGUI req1Text;
    public Image req2;
    public TextMeshProUGUI req2Text;
    public Image req3;
    public TextMeshProUGUI req3Text;
    public GameObject unique;
    public TextMeshProUGUI uniqueText;
    private GameManager gameManager;
    private NewPlayerController playerController;
    public Transform panelTransform;


    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        playerController = GameObject.Find("NewPlayer").GetComponent<NewPlayerController>();
    }

    private void OnEnable()
    {
        isTweening = true;
        isSelected = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 400f, transform.position.z);

        // Get upgradeIndex number
        upgradeIndex = gameManager.selectedNumbers_[index];
        SetDescription();

        // fade in
        panelImage.sprite = spPanelDeselcet;
        transform.DOLocalMoveY(55f, .22f, true).SetUpdate(UpdateType.Normal, true).OnComplete(() =>
        {
            panelTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 5, 1).SetUpdate(true).OnComplete(() =>
            {
                upgradePanelController.isTweening = false;
                AudioManager.Instance.PlaySFX("MouseKlick");
                isTweening = false;

                if (upgradePanelController.selectetPanel == index)
                {
                    if (isSelected == false)
                    {
                        SelectPanel();
                    }
                }
            });
        });
    }




    public void SetDescription()
    {
        if (upgradeChooseList == null)
        {
            upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();
        }

        Upgrade upgrade = upgradeChooseList.uLObject.upgradeList[upgradeIndex];

        // Panel values
        headerText.text = upgrade.headerStr;
        headerText.color = gameManager.cCPrefab.classColor[upgrade.colorIndex];

        iconPanel.sprite = upgrade.iconPanel;

        descriptionText.text = upgrade.descriptionStr;
        
        if (upgradeIndex == 2) // Protection 
        {
            // Get Protection Level
            float currentPercentage = playerController.protectionPerc;
            float normalizedLvl = Mathf.InverseLerp(0, 10, playerController.protectionLvl + 1);
            float targetPercentage = Mathf.RoundToInt(Mathf.Sqrt(normalizedLvl) * 60);
            descriptionText.text = descriptionText.text.Replace("XX", (targetPercentage - currentPercentage).ToString());
            descriptionText.text = descriptionText.text.Replace("YY", (currentPercentage).ToString());
            descriptionText.text = descriptionText.text.Replace("ZZ", (targetPercentage).ToString());
        }

        mainClass.text = upgrade.mainClass.ToString();
        if (mainClass.text == "Nothing") mainClass.text = "";
        mainClass.color = gameManager.cCPrefab.classColor[upgrade.colorIndex];



        // requerments
        // reset
        int count_ = 0;
        req1.color = gameManager.cCPrefab.classColor[8];
        req1Text.text = "";
        req2.color = gameManager.cCPrefab.classColor[8];
        req2Text.text = "";
        req3.color = gameManager.cCPrefab.classColor[8];
        req3Text.text = "";

        Color[] reqColor = new Color[2] { gameManager.cCPrefab.classColor[8], gameManager.cCPrefab.classColor[8] };
        string[] reqText = new string[2] { "", "" };

        // set
        if (upgrade.reqBullet > 0)
        {
            reqColor[0] = gameManager.cCPrefab.classColor[0];
            reqText[0] = upgrade.reqBullet.ToString();
            count_ = 1;
        }
        if (upgrade.reqRocket > 0)
        {
            reqColor[count_] = gameManager.cCPrefab.classColor[1];
            reqText[count_] = upgrade.reqRocket.ToString();
            count_ = 1;
        }
        if (upgrade.reqLaser > 0)
        {
            reqColor[count_] = gameManager.cCPrefab.classColor[2];
            reqText[count_] = upgrade.reqLaser.ToString();
            count_ = 1;
        }
        if (upgrade.reqSupport > 0)
        {
            reqColor[count_] = gameManager.cCPrefab.classColor[3];
            reqText[count_] = upgrade.reqSupport.ToString();
        }


        req1.color = reqColor[0];
        req1Text.text = reqText[0];

        req2.color = reqColor[1];
        req2Text.text = reqText[1];

        if (upgrade.reqAbility != "")
        {
            req3.color = gameManager.cCPrefab.classColor[upgrade.colorIndex];
            req3Text.text = upgrade.reqAbility;
        }
        else
        {
            req3.color = gameManager.cCPrefab.classColor[8];
            req3Text.text = "";
        }

        // Quantity
        if (upgrade.UpgradeCount != 999)
        {
            unique.SetActive(true);
            int upgradeQuanitiy = upgrade.UpgradeCount - upgradeChooseList.upgrades[upgradeIndex].upgradeIndexInstalled;
            uniqueText.text = (upgradeQuanitiy).ToString();
        }
        else
        {
            unique.SetActive(false);
        }
    }

    public void OnMouseEnter_()
    {
        upgradePanelController.selectetPanel = index;

        if (isTweening == false)
        {
            // Farbe des Panels ändern, wenn die Maus über das Panel fährt
            upgradePanelController.UpdateValuePanelOnMouseEnter(index);
        }
    }

    public void SelectPanel()
    {
        isSelected = true;
        panelImage.sprite = spPanelSelect;

        AudioManager.Instance.PlaySFX("MouseHover");

        if (isTweening == false)
        {
            panelTransform.DOComplete();
            panelTransform.DOPunchScale(new Vector3(0.08f, 0.08f, 0.08f), 0.08f, 5, 1).SetUpdate(true);
        }
    }

    public void OnMouseExit_()
    {
        if (isTweening == false)
        {
            // Zurück zur Standardfarbe wechseln, wenn die Maus das Panel verlässt
            upgradePanelController.selectetPanel = -1;
            DeselectPanel();
        }
    }

    public void DeselectPanel()
    {
        isSelected = false;
        panelImage.sprite = spPanelDeselcet;
    }

    public void OnMouseDown_()
    {
        if (isTweening == false && upgradePanelController.isButtonPressed == false)
        {
            if (upgradePanelController.selectetPanel == index)
            {
                panelImage.sprite = spPanelSelect;

                upgradePanelController.ChooseAValue(index);
                AudioManager.Instance.PlaySFX("WindowOpen");

                panelTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), .2f, 5, 1).SetUpdate(true).OnComplete(() => { upgradePanelController.TriggerPanel(index); });

                isTweening = true;
            }
        }
    }

    public void FadeOut(int index_)
    {
        float duration = (float)index / 15;

        if (upgradePanelController.selectetPanel == index)
        {
            panelImage.sprite = spPanelSelect;

            transform.DOLocalMoveY(855f, .7f, true).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InQuart).OnComplete(() =>
            {
                upgradePanelController.GetUpdate();
            });
        }
        else
        {
            transform.DOLocalMoveY(855f, .5f, true).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InQuart).SetDelay(0.2f + duration);
        }
    }
}

