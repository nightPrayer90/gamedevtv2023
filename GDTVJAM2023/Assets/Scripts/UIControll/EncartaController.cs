using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncartaController : MonoBehaviour
{
    public Button button1;
    public int districtIndex = 0;

    public Image districtImage;
    public List<Sprite> districtSprite;

    public TextMeshProUGUI districtTitleText;
    public List<string> districtTitleString;

    public TextMeshProUGUI districtText;
    [TextArea(10, 30)]  public List<string> districtString;

    private void Start()
    {
        UpdateDistrict();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(button1.gameObject);
    }

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
