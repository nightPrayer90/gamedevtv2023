
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconPrefab : MonoBehaviour
{
    [SerializeField] private Image bk_image;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;

    public void SetIcon(int quantityString, Sprite iconSprite, Color bkColor, bool gray = false)
    {
        bk_image.color = bkColor; 
        iconImage.sprite = iconSprite;
        if (gray == true) iconImage.color = Color.gray;
        quantityText.text = quantityString.ToString();
    }

}
