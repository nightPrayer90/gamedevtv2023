using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsModulePrefab : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text moduleNameText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image bkImage;

    [Serializable]
    public struct ModuleDiscriptionContainer
    {
        public TMP_Text disText;
        public TMP_Text disValue;
    }
    [SerializeField] private ModuleDiscriptionContainer[] discription;

    public void SetIconSprite(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }

    public void SetHeaderText(string header, Color color, int quantity)
    {
        moduleNameText.text = header;
        moduleNameText.color = color;

        Debug.Log(quantity);
        if (quantity > 1)
        {
            
            quantityText.text = quantity.ToString();
            quantityText.color = color;
        }
        else
        {
            quantityText.text = "";
        }
        moduleNameText.color = color;
        bkImage.color = color;
    }

    public void SetDescription(int index, string text, string value)
    {
        discription[index].disText.text = text;
        discription[index].disValue.text = value;
    }
}
