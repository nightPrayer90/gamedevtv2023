using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    private Tooltip tooltip;
    
    

    private void Awake()
    {
        current = this;

    }


    public static void Show(Tooltip tooltip_, Vector2 triggerPosition, string content, string header = "")
    {
        current.tooltip = tooltip_;

        current.tooltip.SetText(triggerPosition, content, header);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        current.tooltip.FadeOut();
    }
}
