using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private EventSystem eventsystem;
    [SerializeField] private Button button;
    [SerializeField] private Scrollbar scrollbar;

    // Start is called before the first frame update
    void OnEnable()
    {
        eventsystem.SetSelectedGameObject(gameObject);
        if (button != null) button.Select();
        if (scrollbar != null) scrollbar.Select();
    }
}
