using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public Tooltip tooltip;
    

    private void Awake()
    {
        current = this;
    }

    public static void Show(GameObject triggerObject, string content, string header = "")
    {
        current.tooltip.SetText(triggerObject, content, header);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.FadeOut();
    }
}
