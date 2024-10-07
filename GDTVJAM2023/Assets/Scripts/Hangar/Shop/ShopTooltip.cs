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

public class ShopTooltip : MonoBehaviour
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
            //transform.position = triggerPosition_; // + new Vector2(0f, 20f);
        }
        // FadeIn
        cg.alpha = 0;
        cg.DOFade(1f, 0.05f).SetUpdate(true);
    }
    

    public void ShowFromAbility(Vector2 triggerPosition, int upgradeindex)
    {
        // set tooltip positon
        //triggerPosition_ = triggerPosition;
  
        headerField.text = upgradeList.upgradeList[upgradeindex].headerStr;
        contentField.text = upgradeList.upgradeList[upgradeindex].descriptionStr;

        // layout Element
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit) ? true : false;
    }
}
