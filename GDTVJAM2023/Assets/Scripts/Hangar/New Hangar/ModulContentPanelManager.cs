using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ModulContentPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI nameText;
    public Image imagePanel;
    public Mesh modulMesh;
    public Mesh baseSphereMesh;
    public MeshFilter selectedSphere;
    private Vector3 sphereStartSize;
   
    public void SetText(string text)
    {
        nameText.text = text;
    }

    public void SetPanel(Sprite sprite)
    {
        imagePanel.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.05f, 1.05f, 1.05f), 0.1f);

        sphereStartSize = selectedSphere.transform.localScale;

        if (selectedSphere != null && modulMesh != null)
        {
            selectedSphere.transform.localScale = new Vector3(1f, 1f, 1f);
            selectedSphere.mesh = modulMesh;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOComplete();
        gameObject.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.05f);

        if (selectedSphere != null && modulMesh != null)
        {
            selectedSphere.transform.localScale = sphereStartSize;
            selectedSphere.mesh = baseSphereMesh;
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Panel clicked!");
    }
}
