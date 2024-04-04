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


    void OnEnable()
    {
        if (triggerPosition_ != null)
        {
            transform.position = triggerPosition_; // + new Vector2(0f, 20f);
        }
        // FadeIn
        cg.alpha = 0;
        cg.DOFade(1f, 0.1f).SetUpdate(true);
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
            case 4: // Ship Energie Production
            case 5: // Energie Regen
            case 6: // Energie Storage
            case 7: // Ship mass
            case 8: // Protection
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
}
