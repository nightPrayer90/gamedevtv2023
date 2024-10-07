using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


/*
[System.Serializable]
public class ClassTooltipC
{
    public string header;
    [Multiline]
    public string content;
}
*/

public class HangarTooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWarpLimit;
    private Vector2 triggerPosition_;
    public CanvasGroup cg;

    public ClassTooltipC[] tooltipContent;
    public UpgradeList upgradeList;


    void OnEnable()
    {
        if (triggerPosition_ != null)
        {
            transform.position = triggerPosition_; // + new Vector2(0f, 20f);
        }
        // FadeIn
        cg.alpha = 0;
        cg.DOFade(1f, 0.05f).SetUpdate(true);
    }


    public void Show(Vector2 triggerPosition, int contentType)
    {
           // set tooltip positon
        triggerPosition_ = triggerPosition;

        // set tooltip content
        string header_ = "";
        string content_ = "";
        

        switch (contentType)
        {
            case 0: // bullet Class
            case 1: // explosion Class
            case 2: // laser Class
            case 3: // Support Class
            case 4: // Ability
            case 5: // Energie Regen
            case 6: // Energie Storage
            case 7: // Ship mass
            case 8: // Protection
            case 9: // mainEngine
            case 10:// no Filter
            case 11:// Cockpits
            case 12:// Thruster
            case 13:// Connectors
            case 14:// Weapons
            case 15:// Wings
            case 16:// Direction Engine
            case 17:// Strafe Engine
                header_ = tooltipContent[contentType].header;
                content_ = tooltipContent[contentType].content;
                break;

            default:
                break;
        }

        headerField.text = header_;
        contentField.text = content_;


        // layout Element
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit) ? true : false;
    }

    public void ShowFromAbility(Vector2 triggerPosition, int upgradeindex)
    {
        // set tooltip positon
        triggerPosition_ = triggerPosition;
  
        headerField.text = upgradeList.upgradeList[upgradeindex].headerStr;
        contentField.text = upgradeList.upgradeList[upgradeindex].descriptionStr;

        // layout Element
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit) ? true : false;
    }
}
