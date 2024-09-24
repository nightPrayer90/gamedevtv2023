using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TMP_Text))]
public class DamagePopup : MonoBehaviour
{
    [HideInInspector]
    public TMP_Text tmp_text;
    //public string displayText;

    private void Awake()
    {
        tmp_text = GetComponent<TMP_Text>();
    }


    // Start is called before the first frame update
    private void OnEnable()
    {
        tmp_text.DOComplete();
        tmp_text.DOFade(0f, 1f).From(1f);
        transform.DOMove(transform.position + Vector3.up, 0.8f).OnComplete(() => {
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        });
    }

    public void SetText(string text_)
    {
        tmp_text.text = text_;
    }

}