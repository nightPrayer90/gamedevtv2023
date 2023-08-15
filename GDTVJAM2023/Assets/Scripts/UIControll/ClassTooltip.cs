using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


[System.Serializable]
public class ClassTooltipC
{
    public string header;
    [Multiline]
    public string content;
    public string contentvalue;
}


public class ClassTooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public TextMeshProUGUI contentvalueField;
    public LayoutElement layoutElement;
    public GameManager gameManager;
    public int characterWarpLimit;
    private Vector2 triggerPosition_;
    public CanvasGroup cg;

    public ClassTooltipC[] tooltipContent;

    public Color contentTextValueenabled;
    public Color contentTextValuenull;
    private UpgradeChooseList upgradeChooseList;

    void OnEnable()
    {
        

        if (triggerPosition_ != null)
        {
            transform.position = triggerPosition_;
        }
        // FadeIn
        cg.alpha = 0;
        cg.DOFade(1f, 0.1f).SetUpdate(true);

    }


    public void Show(Vector2 triggerPosition, int contentType)
    {
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();

        // set tooltip positon
        triggerPosition_ = triggerPosition;

        // set tooltip content
        headerField.text = tooltipContent[contentType].header;
        headerField.color = gameManager.globalClassColor[contentType];
        contentField.text = tooltipContent[contentType].content;

        float contentvaluefloat = 0;
        float contentvaluefloat2 = 0;
        string contentvalue_ = "0";

        switch (contentType)
        {
            case 0: // bullet
                contentvaluefloat = upgradeChooseList.baseBulletCritChance;
                contentvaluefloat2 = upgradeChooseList.baseBulletCritDamage;
                contentvalue_ = "Level: " + upgradeChooseList.mcBulletLvl +  " + " + contentvaluefloat.ToString() + "% to crit with " + contentvaluefloat2.ToString() + "% damage";
                break;
            case 1: // explosion
                contentvaluefloat = upgradeChooseList.baseRocketAOERadius;
                contentvalue_ = "Level: " + upgradeChooseList.mcExplosionLvl + "+ " + contentvaluefloat.ToString() + "% more explosion range";
                break;
            case 2: // laser
                contentvaluefloat = upgradeChooseList.baseLaserBurnDamageChance;
                contentvalue_ = "Level: " + upgradeChooseList.mcLaserLvl + "+ " + contentvaluefloat.ToString() + " % burning chance";
                break;
            case 3: // support
                contentvaluefloat = upgradeChooseList.baseSupportRealoadTime;
                contentvalue_ = "Level: " + upgradeChooseList.mcSupportLvl + "- " + contentvaluefloat.ToString() + "% reload time for all weapons";
                break;
            case 4: // swarm
                contentvaluefloat = upgradeChooseList.scSwarmLvl;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + " projectiles";
                break;
            case 5: // defense
                contentvaluefloat = upgradeChooseList.scDefenceLvl * 10;
                contentvalue_ = "- " + contentvaluefloat.ToString() + "%";
                break;
            case 6: // targeting
                contentvaluefloat = upgradeChooseList.scTargetingLvl;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + " damage";
                break;
            case 7: // backwards
                contentvaluefloat = upgradeChooseList.scDirectionLvl;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + " damage";
                break;
        }
        contentvalueField.text = tooltipContent[contentType].contentvalue.Replace("XXX", contentvalue_);
        contentvalueField.color = (contentvaluefloat != 0) ? contentTextValueenabled : contentTextValuenull;


        // layout Element
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit) ? true : false;
    }
}
