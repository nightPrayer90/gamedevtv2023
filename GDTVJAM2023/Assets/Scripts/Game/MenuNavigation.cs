using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private EventSystem eventsystem;
    [SerializeField] private Button button;

    // Start is called before the first frame update
    void OnEnable()
    {
        eventsystem.SetSelectedGameObject(gameObject);
        button.Select();
    }
}
