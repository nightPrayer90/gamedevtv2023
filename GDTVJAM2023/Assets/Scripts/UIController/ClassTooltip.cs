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




[ExecuteInEditMode]
public class ClassTooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public TextMeshProUGUI contentvalueField;
    public LayoutElement layoutElement;
    public int characterWarpLimit;
    private Vector2 triggerPosition_;
    public CanvasGroup cg;

    public ClassTooltipC[] tooltipContent;
    private PlayerWeaponController playerWeaponController;

    public Color contentTextValueenabled;
    public Color contentTextValuenull;

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
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();

        // set tooltip positon
        triggerPosition_ = triggerPosition;

        // set tooltip content
        headerField.text = tooltipContent[contentType].header;
        contentField.text = tooltipContent[contentType].content;

        float contentvaluefloat = 0;
        string contentvalue_ = "0";

        switch (contentType)
        {
            case 0: // bullet
                contentvaluefloat = playerWeaponController.mcBulletLvl * 5;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + "%";
                break;
            case 1: // explosion
                contentvaluefloat = playerWeaponController.mcExplosionLvl * 0.5f;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + "m";
                break;
            case 2: // laser
                contentvaluefloat = playerWeaponController.mcLaserLvl;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + " %";
                break;
            case 3: // support
                contentvaluefloat = playerWeaponController.mcSupportLvl * 3;
                contentvalue_ = "- " + contentvaluefloat.ToString() + "%";
                break;
            case 4: // swarm
                contentvaluefloat = playerWeaponController.scSwarmLvl * 2;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + " projectiles";
                break;
            case 5: // defense
                contentvaluefloat = playerWeaponController.scDefenceLvl * 5;
                contentvalue_ = "- " + contentvaluefloat.ToString() + "%";
                break;
            case 6: // targeting
                contentvaluefloat = playerWeaponController.scTargetingLvl;
                contentvalue_ = "+ " + contentvaluefloat.ToString() + " damage";
                break;
            case 7: // backwards
                contentvaluefloat = playerWeaponController.scBackwardsLvl;
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
