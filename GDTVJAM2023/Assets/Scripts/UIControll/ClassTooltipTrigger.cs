using UnityEngine;



public class ClassTooltipTrigger : MonoBehaviour
{
    private Vector2 triggerPosition;
    public ClassTooltip tooltip;
    public GameObject goTooltip;
    public int contentType = 0;

    private void Start()
    {
        triggerPosition = gameObject.transform.position;
    }

    public void TooltipShow()
    {
        triggerPosition = transform.position;
        tooltip.Show(triggerPosition, contentType);
        goTooltip.SetActive(true);
        AudioManager.Instance.PlaySFX("MouseHover");
    }


    public void TooltipHide()
    {
        //tooltip.Hide();
        goTooltip.SetActive(false);
    }
}
