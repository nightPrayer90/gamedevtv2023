using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class DamagePopup : MonoBehaviour
{
    [HideInInspector]
    //public string displayText;

    // Start is called before the first frame update
    private void OnEnable()
    {
        TMP_Text tmp_text = GetComponent<TMP_Text>();
        tmp_text.DOFade(0f, 1f);
        transform.DOMove(transform.position + Vector3.up, 0.8f).OnComplete(() => {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        });
    }
    public void SetText(string text_)
    {
        TMP_Text tmp_text = GetComponent<TMP_Text>();
        tmp_text.text = text_;
    }
}