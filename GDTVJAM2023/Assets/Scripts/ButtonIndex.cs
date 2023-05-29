using UnityEngine;
using UnityEngine.UI;


public class ButtonIndex : MonoBehaviour
{
    public int buttonIndex;

    private EncartaController encartaController;

    private void Start()
    {
        encartaController = GameObject.Find("Canvas").GetComponent<EncartaController>();
    }

    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX("MouseClick");
        encartaController.districtIndex = buttonIndex;
        encartaController.UpdateDistrict();
    }
}
