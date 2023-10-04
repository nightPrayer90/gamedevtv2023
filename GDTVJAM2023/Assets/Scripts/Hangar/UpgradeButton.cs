using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [Header("Button")]
    public int upgradePanelIndex; // 0=bullet; 1=rocket; 2=laser
    public int upgradeButtonIndex;
    private int colorIndex = -1;
    public int maxButtonLevel;
    private int currentButtonLevel = 0;
    private bool canUpgrade = false;
    public int[] otherUpgradesCosts;
    private int otherUpgradesValue;
    public int[] upgradePointCosts;
    private int upgradePointValue;

    [Header("Button Description")]
    public string tooltipHeaderString;

    [Header("Objects")]
    public Image btnImage;
    public TextMeshProUGUI btnLevelText;
    public PlayerData playerData;
    private HangerUIController hangerUIController;
    private HangarManager gameManager;
    private TooltipTrigger tooltipTrigger;





    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */

    // cash other Objects
    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<HangarManager>();
        hangerUIController = GameObject.Find("Canvas").GetComponent<HangerUIController>();
        tooltipTrigger = gameObject.GetComponent<TooltipTrigger>();
        tooltipTrigger.header = tooltipHeaderString;
    }

    // whenever the button is activated
    private void OnEnable()
    {
        // set all values that we need to controll the upgrade requirements
        RequirementSet();

        // controll the requrements and set the button status
        canUpgrade = RequirementContol();

    }

    private void Update()
    {
        switch (upgradePanelIndex)
        {
            case 0: // bullet
                if (playerData.globalUpgradeCountBullet > otherUpgradesValue)
                {
                    RequirementSet();
                    canUpgrade = RequirementContol();
                }
                break;

            case 1: // rocket
                if (playerData.globalUpgradeCountRocket > otherUpgradesValue)
                {
                    RequirementSet();
                    canUpgrade = RequirementContol();
                }
                break;

            case 2: // laser
                if (playerData.globalUpgradeCountLaser > otherUpgradesValue)
                {
                    RequirementSet();
                    canUpgrade = RequirementContol();
                }
                break;
        }
    }




    /* **************************************************************************** */
    /* Button-Press---------------------------------------------------------------- */
    /* **************************************************************************** */

    // player press the upgrade button
    public void UpgradeAbility()
    {
        if (canUpgrade == true)
        {
            AudioManager.Instance.PlaySFX("MouseKlick");

            switch (upgradePanelIndex)
            {
                case 0: // bullet
                    playerData.expBullet -= upgradePointCosts[currentButtonLevel];
                    hangerUIController.SetUpgradePointText(playerData.expBullet);

                    currentButtonLevel += 1;
                    playerData.globalUpgradeCountBullet += 1;
                    playerData.bulletResearchedSkills[upgradeButtonIndex] = currentButtonLevel;
                    break;

                case 1: // rocket
                    playerData.expRocket -= upgradePointCosts[currentButtonLevel];
                    hangerUIController.SetUpgradePointText(playerData.expRocket);

                    currentButtonLevel += 1;
                    playerData.globalUpgradeCountRocket += 1;
                    playerData.rocketResearchedSkills[upgradeButtonIndex] = currentButtonLevel;
                    break;

                case 2: // laser
                    playerData.expLaser -= upgradePointCosts[currentButtonLevel];
                    hangerUIController.SetUpgradePointText(playerData.expLaser);

                    currentButtonLevel += 1;
                    playerData.globalUpgradeCountLaser += 1;
                    playerData.laserResearchedSkills[upgradeButtonIndex] = currentButtonLevel;
                    break;
            }

            tooltipTrigger.Hide();
            BuildUpgradeText();

            // set new ship data values
            gameManager.SetShipDataValues(upgradePanelIndex);

            // check the button for new updates
            RequirementSet();

            Debug.Log(currentButtonLevel + " - " + maxButtonLevel);
            canUpgrade = RequirementContol();

            tooltipTrigger.Refresh();
        }
        else
        {
            AudioManager.Instance.PlaySFX("MouseNo");
        }
    }

    /* **************************************************************************** */
    /* Help-Press---------------------------------------------------------------- */
    /* **************************************************************************** */

    // can the skill be upgraded?
    public void RequirementSet()
    {
        switch (upgradePanelIndex)
        {
            case 0: // bullet
                currentButtonLevel = playerData.bulletResearchedSkills[upgradeButtonIndex];

                otherUpgradesValue = playerData.globalUpgradeCountBullet;
                upgradePointValue = playerData.expBullet;
                colorIndex = 0;
                break;

            case 1: // rocket
                currentButtonLevel = playerData.rocketResearchedSkills[upgradeButtonIndex];

                otherUpgradesValue = playerData.globalUpgradeCountRocket;
                upgradePointValue = playerData.expRocket;
                colorIndex = 1;
                break;

            case 2: // laser
                currentButtonLevel = playerData.laserResearchedSkills[upgradeButtonIndex];

                otherUpgradesValue = playerData.globalUpgradeCountLaser;
                upgradePointValue = playerData.expLaser;
                colorIndex = 2;
                break;
        }

        BuildUpgradeText();
    }

    // set the button status
    public bool RequirementContol()
    {
        bool canUpgrade_ = false;

        if (currentButtonLevel < maxButtonLevel)
        {
            // button cost requierments
            if (otherUpgradesValue >= otherUpgradesCosts[currentButtonLevel])
            {
                if (upgradePointValue >= upgradePointCosts[currentButtonLevel])
                {
                    btnImage.color = Color.white;
                    tooltipTrigger.content = "cost " + upgradePointCosts[currentButtonLevel] + " upgrade points";
                    canUpgrade_ = true;
                }
                else
                {
                    btnImage.color = Color.gray;
                    tooltipTrigger.content = "cost " + upgradePointCosts[currentButtonLevel] + " upgrade points";
                    canUpgrade_ = false;
                }
            }
            else
            {
                btnImage.color = Color.gray;
                tooltipTrigger.content = "need " + (otherUpgradesCosts[currentButtonLevel] - otherUpgradesValue).ToString() + " other upgrades";
                canUpgrade_ = false;
            }
        }
        // button is allready upgraded - and cant be upgraded again
        if (currentButtonLevel == maxButtonLevel) 
        {
            btnImage.color = gameManager.classColors[colorIndex];
            tooltipTrigger.content = "";
            canUpgrade_ = false;
        }

        return canUpgrade_;
    }

    // set the text "1 /1"
    public void BuildUpgradeText()
    {
        btnLevelText.text = currentButtonLevel.ToString() + "/" + maxButtonLevel.ToString();
    }
}
