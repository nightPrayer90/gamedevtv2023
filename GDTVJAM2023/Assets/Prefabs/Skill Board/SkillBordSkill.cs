using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;


public class SkillBordSkill : MonoBehaviour
{
    private SkillBordController controller;
    public int skillIndex = 0;

    [Header("UI-Controll")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI discriptionText;
    public Image icoSprite;
    public TextMeshProUGUI mainClassText;

    public GameObject[] regContainer;
    public TextMeshProUGUI[] reqText;
    public Image[] reqImage;

    public GameObject countContainer;
    public TextMeshProUGUI count_text;

    private int colorIndex = 0;


    private void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<SkillBordController>();
        colorIndex = controller.ulPrefab.upgradeList[skillIndex].colorIndex;

        headerText.text = controller.ulPrefab.upgradeList[skillIndex].headerStr.ToString();
        headerText.color = controller.ccPrefab.classColor[colorIndex];

        discriptionText.text = controller.ulPrefab.upgradeList[skillIndex].descriptionStr.ToString();
        icoSprite.sprite = controller.ulPrefab.upgradeList[skillIndex].iconPanel;
        
        mainClassText.text = controller.ulPrefab.upgradeList[skillIndex].mainClass.ToString() == "Nothing" ? "" : controller.ulPrefab.upgradeList[skillIndex].mainClass.ToString();
        
        switch (controller.ulPrefab.upgradeList[skillIndex].mainClass.ToString())
        {
            case "Bullet":
                mainClassText.color = controller.ccPrefab.classColor[0];
                break;
            case "Rocket":
                mainClassText.color = controller.ccPrefab.classColor[1];
                break;
            case "Laser":
                mainClassText.color = controller.ccPrefab.classColor[2];
                break;
            case "Support":
                mainClassText.color = controller.ccPrefab.classColor[3];
                break;
            default:
                mainClassText.color = controller.ccPrefab.classColor[colorIndex];
                break;
        }

        // TODO Color
        int index = 0;
        if (controller.ulPrefab.upgradeList[skillIndex].reqBullet != 0)
        {
            regContainer[index].SetActive(true);
            reqText[index].text = controller.ulPrefab.upgradeList[skillIndex].reqBullet.ToString();
            reqImage[index].color = controller.ccPrefab.classColor[0]; 
            index++;
        }
        if (controller.ulPrefab.upgradeList[skillIndex].reqRocket != 0)
        {
            regContainer[index].SetActive(true);
            reqText[index].text = controller.ulPrefab.upgradeList[skillIndex].reqRocket.ToString();
            reqImage[index].color = controller.ccPrefab.classColor[1];
            index++;
        }
        if (controller.ulPrefab.upgradeList[skillIndex].reqLaser != 0)
        {
            regContainer[index].SetActive(true);
            reqText[index].text = controller.ulPrefab.upgradeList[skillIndex].reqLaser.ToString();
            reqImage[index].color = controller.ccPrefab.classColor[2];
            index++;
        }
        if (controller.ulPrefab.upgradeList[skillIndex].reqAbility != "")
        {
            regContainer[2].SetActive(true);
            reqText[2].text = controller.ulPrefab.upgradeList[skillIndex].reqAbility;
            reqImage[2].color = controller.ccPrefab.classColor[colorIndex];
        }

        // Count Container
        if (controller.ulPrefab.upgradeList[skillIndex].UpgradeCount != 999)
        {
            countContainer.SetActive(true);
            count_text.text = controller.ulPrefab.upgradeList[skillIndex].UpgradeCount.ToString();
        }
    }
}
