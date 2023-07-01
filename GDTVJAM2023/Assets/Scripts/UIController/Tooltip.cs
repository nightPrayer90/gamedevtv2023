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
    private GameObject triggerObejct_;
    public CanvasGroup cg;

    //public Animator animator;

    /*public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
        //animator.Play("FadeCanvasGroup");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
        //animator.Play("FadeOutCanvsGroup");
    }*/

    void OnEnable()
    {
        if (triggerObejct_ != null)
        {
            Vector2 position = triggerObejct_.transform.position;

            transform.position = position;
        }
        cg.alpha = 0;
        cg.DOFade(1f, 0.2f).SetUpdate(true);
        AudioManager.Instance.PlaySFX("MouseHover");
    }

    public void FadeOut()
    {
        //cg.DOFade(0f, 0.02f).SetUpdate(true).From(1).OnComplete(() =>
        //{
            gameObject.SetActive(false);
        //});
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


    public void SetText(GameObject triggerObject, string content, string header = "")
    {
        triggerObejct_ = triggerObject;

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
