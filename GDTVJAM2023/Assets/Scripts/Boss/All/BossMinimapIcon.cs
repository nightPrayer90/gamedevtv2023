using DG.Tweening;
using UnityEngine;

public class BossMinimapIcon : MonoBehaviour
{
    private SpriteRenderer minimapSpR;

    public void Start()
    {
        minimapSpR = gameObject.GetComponent<SpriteRenderer>();
    }

    public void InitMinimapIcon()
    {
        minimapSpR.DOFade(1f, 2f).SetDelay(1f);
        gameObject.transform.DOScale(new Vector3(15f, 15f, 15f), 2f).SetDelay(1f).OnComplete(() =>
        {
            gameObject.transform.DOPunchScale(new Vector3(7f, 7f, 7f), 2f, 1, 0.4f).SetDelay(2f).SetLoops(-1, LoopType.Restart);
        });
    }

    public void SetIconToRed()
    {
        // set minimap to a red color
        gameObject.transform.DOComplete();
        gameObject.transform.DOKill();
        minimapSpR.DOColor(Color.red, 1f);
    }

    public void HideMinimapIcon()
    {
        minimapSpR.DOFade(0f, 0.5f);
    }
}
