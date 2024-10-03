
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconPrefab : MonoBehaviour
{
    [SerializeField] private Image bk_image;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;
    public int upgradeindex = 0;

    public void SetIcon(int quantityString, Sprite iconSprite, Color bkColor, int upgradeindex, bool gray = false)
    {
        this.upgradeindex = upgradeindex;
        bk_image.color = bkColor; 
        iconImage.sprite = iconSprite;
        if (gray == true) iconImage.color = Color.gray;
        quantityText.text = quantityString.ToString();
    }

}
