using UnityEngine;



public class ClassTooltipTrigger : MonoBehaviour
{
    private Vector2 triggerPosition;
    public ClassTooltip tooltip;
    public GameObject goTooltip;
    public int contentType = 0;
    public Vector3 toolTipOffset;
    public IconPrefab iconPrefab;

    private void Start()
    {
        triggerPosition = gameObject.transform.position;

        if (goTooltip == null)
        {
            tooltip = GameObject.FindObjectOfType<ClassTooltip>(true);
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

    public void AbilityTooltipShow()
    {
        triggerPosition = transform.position + toolTipOffset;
        tooltip.ShowAbilityTooltip(triggerPosition, iconPrefab.upgradeindex);
        goTooltip.SetActive(true);
        AudioManager.Instance.PlaySFX("MouseHover");

    }


    public void TooltipHide()
    {
        //tooltip.Hide();
        goTooltip.SetActive(false);
    }
}
