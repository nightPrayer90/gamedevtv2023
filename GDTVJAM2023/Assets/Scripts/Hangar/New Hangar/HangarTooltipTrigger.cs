using UnityEngine;



public class HangarTooltipTrigger : MonoBehaviour
{
    private Vector2 triggerPosition;
    public HangarTooltip tooltip;
    public GameObject goTooltip;
    public int contentType = 0;
    public Vector3 toolTipOffset;

    private void Start()
    {
        triggerPosition = gameObject.transform.position;
    }

    public void TooltipShow()
    {
        if (contentType != -1)
        {
            triggerPosition = transform.position + toolTipOffset;
            tooltip.Show(triggerPosition, contentType);
            goTooltip.SetActive(true);
            AudioManager.Instance.PlaySFX("MouseHover");
        }
    }


    public void TooltipHide()
    {
        //tooltip.Hide();
        goTooltip.SetActive(false);
    }
}
