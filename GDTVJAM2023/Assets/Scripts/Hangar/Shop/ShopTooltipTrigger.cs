using UnityEngine;

public class ShopTooltipTrigger : MonoBehaviour
{
    private Vector2 triggerPosition;
    
    public ShopTooltip tooltip;
    public GameObject goTooltip;

    public int contentType = 0;
    public Vector3 toolTipOffset;


    private void Start()
    {
        triggerPosition = gameObject.transform.position;

        tooltip = FindObjectOfType<ShopTooltip>(true);
        goTooltip = tooltip.gameObject;
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
