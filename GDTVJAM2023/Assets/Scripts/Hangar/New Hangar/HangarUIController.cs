using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class HangarUIController : MonoBehaviour
{
    [Header("Scene Management")]
    public string gameScene = "GameScene";
    public string menueScene = "MenueScene";
    public string shopScene = "ShopScene";
    public string skillBoardScene = "SkillBordScene";

    [Header("UI Controls")]
    public CanvasGroup modulePanel;
    public CanvasGroup removePanel;
    public CanvasGroup removeUnconnectedPanel;
    public CanvasGroup selectionContentPanel;
    public CanvasGroup mouseOverPanel;
    public CanvasGroup notificationPanel;
    public CanvasGroup newModuleNotification;

    [Header("Selection Content Panel")]
    public TextMeshProUGUI scpHeader;
    public TextMeshProUGUI scpDescription;
    public TextMeshProUGUI scpCostMassValue;
    public TextMeshProUGUI scpCostEnergieValue;
    public Transform contentIconParent;

    [Header("Mouse Over Panal")]
    public TextMeshProUGUI mopHeader;
    public TextMeshProUGUI mopDescription;
    public TextMeshProUGUI mopCostMassValue;
    public TextMeshProUGUI mopCostEnergieValue;
    public Transform mouseOverIconParent;
    public Image mopAbilitsSprite;
    public GameObject mopAbilitsPanel;

    [Header("Ship Panel")]
    public TextMeshProUGUI spMassValue;
    public TextMeshProUGUI spEnergieProduction;
    public TextMeshProUGUI spEnergieRegen;
    public TextMeshProUGUI spEnergieStorage;
    public TextMeshProUGUI spHealth;
    public TextMeshProUGUI spProtection;
    public TextMeshProUGUI spMainEngine;
    public TextMeshProUGUI spDirectionEngine;
    public TextMeshProUGUI spStrafeEngine;
    public Image spMainEngineImage;
    public Image spDirectionEngineImage;
    public Image spStrafeEngineImage;

    [Header("Nion Panel")]
    public GameObject nionPrefab;
    public Transform nionPanel;
    private List<Image> nionImages = new();

    [Header("Upgrade Panel")]
    public GameObject iconPrefab;
    public Transform layoutParent;
    public UpgradeList upgradeList;
    public ClassColor classColor;

    [Header("Class Panel")]
    public Image cpBulletImage;
    public Image cpRocketImage;
    public Image cpLaserImage;
    public Image cpSupportImage;
    public TextMeshProUGUI cpBulletText;
    public TextMeshProUGUI cpRocketText;
    public TextMeshProUGUI cpLaserText;
    public TextMeshProUGUI cpSupportText;

    [Header("Notification Panel")]
    public TextMeshProUGUI notificationText;

    [Header("Game Objects")]
    public ModuleStorage moduleStorage;
    public Transform contentParent;
    private HangarSelection selectionController;
    public HangarFilterBtn hangarFilterBtn;
    public Image abilityPanel;


    private void Awake()
    {
        // if the player comes from the Gamescene Time.timeScale = 0;
        Time.timeScale = 1;
    }

    private void Start()
    {
        selectionController = gameObject.GetComponent<HangarSelection>();
        selectionController.OnDeselect += HandleDeselect;
        modulePanel.alpha = 0;
        modulePanel.blocksRaycasts = false;
        removePanel.alpha = 0;
        removePanel.blocksRaycasts = false;
        removeUnconnectedPanel.alpha = 0;
        removeUnconnectedPanel.blocksRaycasts = false;
        selectionContentPanel.alpha = 0;
        selectionContentPanel.blocksRaycasts = false;
        mouseOverPanel.alpha = 0;
        mouseOverPanel.blocksRaycasts = false;
        notificationPanel.alpha = 0;

        cpBulletImage.enabled = false;
        cpRocketImage.enabled = false;
        cpLaserImage.enabled = false;
        cpSupportImage.enabled = false;

        if (moduleStorage.playerData.shopLevelVisited != moduleStorage.playerData.bossLevel)
        {
            newModuleNotification.DOFade(0.0f, 1f).SetEase(Ease.InQuint).SetLoops(-1, LoopType.Restart);
        }
        else
        {
            newModuleNotification.alpha = 0;
        }
    }

    private void OnDestroy()
    {
        selectionController.OnDeselect -= HandleDeselect;
    }

    // handle Sphere selection
    public void HandleShpereSelect(Sphere selection)
    {
        // load Content Panel
        if (selection != null)
        {
            int modules = 0;
            MeshFilter mRSph = selection.GetComponent<MeshFilter>();

            modules = moduleStorage.possibleModules.Count;

            if (modules > 0)
            {
                AudioManager.Instance.PlaySFX("HangarSelectSphere");

                // open Panel
                modulePanel.DOKill();
                if (modulePanel.alpha != 1)
                {
                    modulePanel.blocksRaycasts = true;
                    modulePanel.DOFade(1, 0.2f);
                }

                // duplicate Content Moduls
                hangarFilterBtn.moduleParentTransform = selection.parentTransform;
                hangarFilterBtn.BuildLists();
                hangarFilterBtn.CreateFromHangarUIController(contentParent, mRSph);
            }
            else
            {
                AudioManager.Instance.PlaySFX("HangarCantLoadSelect");
            }
        }
    }

    // handle Module selection
    public void HandleModulSelect(HangarModul selectedModul, bool selectSound = true)
    {
        // Handle Panel UI
        removePanel.DOKill();
        selectionContentPanel.DOKill();
        if (removePanel.alpha != 1 && selectedModul.moduleValues.moduleType != ModuleType.Cockpit) //TODO hasNoParentControl - cant delete Cockpit or StrafeEngine
        {
            removePanel.blocksRaycasts = true;
            removePanel.DOFade(1, 0.2f);
        }
        if (removePanel.alpha == 1 && selectedModul.moduleValues.moduleType == ModuleType.Cockpit)
        {
            removePanel.blocksRaycasts = false;
            removePanel.DOFade(0, 0.2f);
        }


        if (selectionContentPanel.alpha != 1)
        {
            selectionContentPanel.blocksRaycasts = true;
            selectionContentPanel.DOFade(1, 0.2f);
        }

        // set content selectionPanel
        scpHeader.text = selectedModul.moduleValues.moduleName;
        scpDescription.text = selectedModul.moduleValues.modulDescription_multiLineText;
        scpCostMassValue.text = selectedModul.moduleValues.costMass.ToString() + " t";
        scpCostEnergieValue.text = selectedModul.moduleValues.costEnergie.ToString() + " Nion";


        int iconUpgradArr = selectedModul.moduleValues.moduleUpgrades.Count;

        // Icon Panel
        foreach (Transform child in contentIconParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < iconUpgradArr; i++)
        {
            int upgradeIndex = selectedModul.moduleValues.moduleUpgrades[i].moduleUpgradeIndex;
            int upgradequantity = selectedModul.moduleValues.moduleUpgrades[i].moduleUpgradeQuantity;

            if (upgradequantity > 0)
            {
                GameObject go = Instantiate(iconPrefab, contentIconParent);
                IconPrefab iPf = go.GetComponent<IconPrefab>();
                iPf.SetIcon(upgradequantity, upgradeList.upgradeList[upgradeIndex].iconPanel, classColor.classColor[upgradeList.upgradeList[upgradeIndex].colorIndex], upgradeIndex, false);
            }
        }



        // open Module Panel
        int modules = moduleStorage.possibleModules.Count;

        if (selectSound == true) AudioManager.Instance.PlaySFX("HangarSelectPart");

        if (modules > 0)
        {
            Debug.Log(scpHeader.text = selectedModul.moduleValues.moduleName);
            modulePanel.DOKill();
            if (modulePanel.alpha != 1)
            {
                modulePanel.blocksRaycasts = true;
                modulePanel.DOFade(1, 0.2f);
            }

            // duplicate Content Moduls
            hangarFilterBtn.moduleParentTransform = selectedModul.transform;
            hangarFilterBtn.BuildLists();
            hangarFilterBtn.CreateFromHangarUIController(contentParent);
        }
    }

    // deselect all
    public void HandleDeselect()
    {
        modulePanel.DOFade(0, 0.2f).OnComplete(() => { modulePanel.blocksRaycasts = false; });
        removePanel.DOFade(0, 0.2f).OnComplete(() => { removePanel.blocksRaycasts = false; });
        selectionContentPanel.DOFade(0, 0.2f).OnComplete(() => { selectionContentPanel.blocksRaycasts = true; });
    }

    public void ControllUnconnectedModules(bool isAllConnected)
    {
        removeUnconnectedPanel.DOKill();

        if (isAllConnected == false)
        {
            removeUnconnectedPanel.DOFade(1, 0.2f).OnComplete(() => { removeUnconnectedPanel.blocksRaycasts = true; });
        }
        else
        {
            removeUnconnectedPanel.DOFade(0, 0.2f).OnComplete(() => { removeUnconnectedPanel.blocksRaycasts = false; });
        }
    }


    /* **************************************************************************** */
    /* Mouse Over Panel------------------------------------------------------------ */
    /* **************************************************************************** */
    #region mouse over Panel
    public void MouseOverModulePanel(int modulIndex)
    {
        mouseOverPanel.DOKill();
        if (mouseOverPanel.alpha != 1)
        {
            mouseOverPanel.blocksRaycasts = true;
            mouseOverPanel.DOFade(1, 0.2f);
        }

        // set content Mouse over Panel
        mopHeader.text = moduleStorage.moduleList.moduls[modulIndex].moduleName;
        mopDescription.text = moduleStorage.moduleList.moduls[modulIndex].moduleValues.modulDescription_multiLineText;
        mopCostMassValue.text = moduleStorage.moduleList.moduls[modulIndex].moduleValues.costMass.ToString() + " t";
        mopCostEnergieValue.text = moduleStorage.moduleList.moduls[modulIndex].moduleValues.costEnergie.ToString() + " Nion";
        
        int iconUpgradArr = moduleStorage.moduleList.moduls[modulIndex].moduleValues.moduleUpgrades.Count;

        // Icon Panel
        foreach (Transform child in mouseOverIconParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < iconUpgradArr; i++)
        {
            int upgradeIndex = moduleStorage.moduleList.moduls[modulIndex].moduleValues.moduleUpgrades[i].moduleUpgradeIndex;
            int upgradequantity = moduleStorage.moduleList.moduls[modulIndex].moduleValues.moduleUpgrades[i].moduleUpgradeQuantity;

            if (upgradequantity > 0)
            {
                GameObject go = Instantiate(iconPrefab, mouseOverIconParent);
                IconPrefab iPf = go.GetComponent<IconPrefab>();
                iPf.SetIcon(upgradequantity, upgradeList.upgradeList[upgradeIndex].iconPanel,  classColor.classColor[upgradeList.upgradeList[upgradeIndex].colorIndex] , upgradeIndex, false);
            }
        }

        // ability Panel
        if (moduleStorage.moduleList.moduls[modulIndex].abilitySprite != null)
        {
            mopAbilitsPanel.SetActive(true);
            mopAbilitsSprite.sprite = moduleStorage.moduleList.moduls[modulIndex].abilitySprite;
        }
        else
        {
            mopAbilitsPanel.SetActive(false) ;
        }
    }

    public void MouseExitModulePanel(float delay)
    {
        mouseOverPanel.DOKill();
        if (mouseOverPanel.alpha != 0)
        {
            mouseOverPanel.blocksRaycasts = false;
            mouseOverPanel.DOFade(0, 0.2f).SetDelay(delay);
        }
    }

    #endregion



    /* **************************************************************************** */
    /* SHIP PANEL------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region Ship Panel
    public void SetShipPanel()
    {
        int nion = 0;
        float massResult = 0f;
        float energieProduction = 0f;
        float nionCost = 0f;
        int energieStorage = 0;
        int health = 0;
        float protection = 0;
        float mainEngine = 0;
        float strafeEngine = 0;
        float directionEngine = 0;
        float boostEngine = 0;
        float boostStrafe = 0;
        int bulletClass = 0;
        int rocketClass = 0;
        int laserClass = 0;
        int supportClass = 0;


        int[] iconUpgradArr = new int[upgradeList.upgradeList.Count];
        // get Data
        foreach (HangarModul modul in moduleStorage.installedHangarModules)
        {
            nion += modul.moduleValues.nion;     //-> Nion

            nionCost += modul.moduleValues.costEnergie;              //-> kosten Nion
            energieStorage += modul.moduleValues.energieStorage;
            energieProduction += modul.moduleValues.energieProduction;

            massResult += modul.moduleValues.costMass;
            health += modul.moduleValues.health;
            protection += modul.moduleValues.protection;
            mainEngine += modul.moduleValues.mainEngine;
            strafeEngine += modul.moduleValues.strafeEngine;
            directionEngine += modul.moduleValues.directionEngine;
            boostEngine += modul.moduleValues.boostEngine;
            boostStrafe += modul.moduleValues.boostStrafeEngine;
            bulletClass += modul.moduleValues.bulletClass;
            rocketClass += modul.moduleValues.rocketClass;
            laserClass += modul.moduleValues.laserClass;
            supportClass += modul.moduleValues.supportClass;


            foreach (var item in modul.moduleValues.moduleUpgrades)
            {
                iconUpgradArr[item.moduleUpgradeIndex] += item.moduleUpgradeQuantity;
            }
        }

        // Nion Panel
        // Delete all NionPrefabs
        foreach (Transform child in nionPanel)
        {
            GameObject.Destroy(child.gameObject);
        }


        moduleStorage.isEnergiePositiv = nionCost > nion ? false : true;

        for (int i = 0; i < nion; i++)
        {
            GameObject go = Instantiate(nionPrefab, nionPanel);

            if (i < nionCost)
            {
                if (moduleStorage.isEnergiePositiv)
                    go.GetComponent<Image>().color = Color.white;
                else
                    go.GetComponent<Image>().color = Color.red;
            }
        }


        // Ship Panel
        spEnergieProduction.text = nion.ToString();
        spEnergieRegen.text = energieProduction.ToString() + " TJ/s"; ; // -> Value that comes from oduleValues
        spEnergieStorage.text = energieStorage.ToString() + " TJ";

        spMassValue.text = massResult.ToString() + " t";
        spMainEngine.text = mainEngine.ToString() + " /<color=#00FFFF>" + boostEngine.ToString() + "</color> TN";
        directionEngine = Mathf.Round((directionEngine / 2) * 100) / 100;
        spDirectionEngine.text = directionEngine.ToString() + " TNm";
        spStrafeEngine.text = strafeEngine.ToString() + " /<color=#00FFFF>" + boostStrafe.ToString() + "</color> TN";

        if (mainEngine <= 0) spMainEngineImage.color = Color.red;
        else spMainEngineImage.color = Color.white;

        if (directionEngine <= 0) spDirectionEngineImage.color = Color.red;
        else spDirectionEngineImage.color = Color.white;

        if (strafeEngine <= 0) spStrafeEngineImage.color = Color.red;
        else spStrafeEngineImage.color = Color.white;

        spHealth.text = health.ToString() + " HP";

        protection = Mathf.InverseLerp(0, 10, protection);
        protection = Mathf.RoundToInt(Mathf.Sqrt(protection) * 60);

        spProtection.text = protection.ToString() + " %";


        // Class Panel
        cpBulletImage.enabled = (bulletClass > 0) ? true : false;
        cpBulletText.text = bulletClass.ToString();
        cpRocketImage.enabled = (rocketClass > 0) ? true : false;
        cpRocketText.text = rocketClass.ToString();
        cpLaserImage.enabled = (laserClass > 0) ? true : false;
        cpLaserText.text = laserClass.ToString();
        cpSupportImage.enabled = (supportClass > 0) ? true : false;
        cpSupportText.text = supportClass.ToString();

        // Icon Panel
        foreach (Transform child in layoutParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < iconUpgradArr.Length; i++)
        {
            if (iconUpgradArr[i] > 0)
            {
                GameObject go = Instantiate(iconPrefab, layoutParent);
                IconPrefab iPf = go.GetComponent<IconPrefab>();
                iPf.SetIcon(iconUpgradArr[i], upgradeList.upgradeList[i].iconPanel, classColor.classColor[upgradeList.upgradeList[i].colorIndex], i, false);
            }
        }
    }


    #endregion



    /* **************************************************************************** */
    /* BUTTON CONTOLS-------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Button Controls
    public void GameStart()
    {
        Debug.Log($"Game can Start? {moduleStorage.isEnergiePositiv}");

        if (moduleStorage.isAllConnected == true && moduleStorage.isEnergiePositiv == true)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");
            AudioManager.Instance.SceneTransition(gameScene);
            //SceneManager.LoadScene(gameScene);
        }
        else
        {
            if (moduleStorage.isEnergiePositiv == false)
            {
                notificationText.text = "Your ship needs more Nion power to start!";
                AudioManager.Instance.PlaySFX("CantStartGame1");
            }

            if (moduleStorage.isAllConnected == false)
            {
                notificationText.text = "All modules must be connected!";
                AudioManager.Instance.PlaySFX("CantStartGame2");
            }

            notificationPanel.alpha = 1;
            notificationPanel.DOKill();
            notificationPanel.DOFade(0, 3f).SetDelay(1f);
        }
    }

    public void BackToMenue()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        //SceneManager.LoadScene(menueScene);
        AudioManager.Instance.SceneTransition(menueScene, 1);
    }

    public void GoToShop()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        AudioManager.Instance.SceneTransition(shopScene);
        //SceneManager.LoadScene(shopScene);
    }
    public void GoToSkillBoard()
    {
        AudioManager.Instance.PlaySFX("MouseKlick");
        AudioManager.Instance.SceneTransition(skillBoardScene);
        //SceneManager.LoadScene(skillBoardScene);
    }

    #endregion
}
