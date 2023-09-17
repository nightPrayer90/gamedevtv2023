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
}


public class ClassTooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
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
            transform.position = triggerPosition_ + new Vector2(0f, 20f);
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

       

        float contentvaluefloat = 0;
        string content_ = tooltipContent[contentType].content;

        switch (contentType)
        {
            case 0: // bullet
                content_.Replace("XXX", upgradeChooseList.percBulletDamage.ToString());
                content_.Replace("YYY", upgradeChooseList.baseBulletCritChance.ToString());
                content_.Replace("ZZZ", upgradeChooseList.baseBulletCritDamage.ToString());
                break;
            case 1: // explosion

                break;
            case 2: // laser

                break;
            case 3: // support
                //contentvaluefloat = upgradeChooseList.baseSupportRealoadTime;
                //contentvalue_ = "Level: " + upgradeChooseList.mcSupportLvl + "- " + contentvaluefloat.ToString() + "% reload time for all weapons";
                break;
        }

        contentField.text = content_;
        //contentvalueField.text = tooltipContent[contentType].contentvalue.Replace("XXX", contentvalue_);
        //contentvalueField.color = (contentvaluefloat != 0) ? contentTextValueenabled : contentTextValuenull;


        // layout Element
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit) ? true : false;
    }
}
