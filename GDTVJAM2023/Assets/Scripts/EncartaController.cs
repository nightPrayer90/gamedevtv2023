using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncartaController : MonoBehaviour
{
    public int districtIndex = 0;

    public Image districtImage;
    public List<Sprite> districtSprite;

    public TextMeshProUGUI districtTitleText;
    public List<string> districtTitleString;

    public TextMeshProUGUI districtText;
    [TextArea(10, 30)]  public List<string> districtString;

    public void UpdateDistrict()
    {
        // header
        districtTitleText.text = districtTitleString[districtIndex];

        // Sprite
        districtImage.sprite = districtSprite[districtIndex];

        // body
        districtText.text = districtString[districtIndex];
    }

}
