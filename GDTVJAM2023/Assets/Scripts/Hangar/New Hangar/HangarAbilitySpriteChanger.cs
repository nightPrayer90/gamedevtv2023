using UnityEngine;
using UnityEngine.UI;


public class HangarSpriteChanger : MonoBehaviour
{
    private HangarUIController hangarUIController;
    public Sprite abilityImage;

    private void Start()
    {
        hangarUIController = GameObject.Find("SelectionController").GetComponent<HangarUIController>();
        hangarUIController.abilityPanel.sprite = abilityImage;
    }
}
