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

        if (goTooltip == null)
        {
            tooltip = GameObject.FindObjectOfType<HangarTooltip>(true);
            goTooltip = tooltip.gameObject;
        }
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

    public void TooltipShowAbility()
    {
        IconPrefab iconPrefab = gameObject.GetComponent<IconPrefab>();
        triggerPosition = transform.position + toolTipOffset;
        
        tooltip.ShowFromAbility(triggerPosition, iconPrefab.upgradeindex);
        goTooltip.SetActive(true);
        AudioManager.Instance.PlaySFX("MouseHover");
    }


    public void TooltipHide()
    {
        //tooltip.Hide();
        goTooltip.SetActive(false);
    }
}
