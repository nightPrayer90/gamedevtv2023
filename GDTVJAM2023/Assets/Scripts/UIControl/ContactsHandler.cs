using UnityEngine;
using TMPro;

public class ContactsHandler : MonoBehaviour//, IPointerClickHandler
{
    public TextMeshProUGUI TextMeshPro;
    public Canvas Canvas;
    public Camera Camera;

    private void Awake()
    {
        TextMeshPro.GetComponent<TextMeshProUGUI>();
        Camera = Canvas.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay ? null : Canvas.worldCamera;
    }

    private void Update()
    {
        // ReSharper disable once InvertIf
        if (Input.GetMouseButtonDown(0))
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(TextMeshPro.GetComponent<TextMeshProUGUI>(), Input.mousePosition, Camera);
            Debug.Log(linkIndex);
            if (linkIndex == -1) return;
            var linkInfo = TextMeshPro.textInfo.linkInfo[linkIndex];

            switch (linkInfo.GetLinkID())
            {
                case "tel1":
                    Application.OpenURL("https://discord.com/invite/CrBvxFb4jc");
                    linkInfo.textComponent.color = Color.black;
                    break;
                case "tel2":
                    Application.OpenURL("tel://+74955323002");
                    break;
                case "site":
                    Application.OpenURL("http://orange-servis.ru");
                    break;
                case "email":
                    Application.OpenURL("mailto://hi@orange-servis.ru");
                    break;
                default:
                    break;
            }
        }
    }
}