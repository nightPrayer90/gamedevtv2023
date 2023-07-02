using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;



[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWarpLimit;
    private Vector2 triggerPosition_;
    public CanvasGroup cg;

    

    void OnEnable()
    {
        if (triggerPosition_ != null)
        {
            transform.position = triggerPosition_;
        }
        // FadeIn
        cg.alpha = 0;
        cg.DOFade(1f, 0.1f).SetUpdate(true);
        AudioManager.Instance.PlaySFX("MouseHover");
    }

    public void FadeOut()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;

            layoutElement.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit) ? true : false;
       
        }
    }


    public void SetText(Vector2 triggerPosition, string content, string header = "")
    {
        triggerPosition_ = triggerPosition;

        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;
    }
}
