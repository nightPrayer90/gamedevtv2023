using UnityEngine;
using DG.Tweening;

public class HangarShipTransform : MonoBehaviour
{
    Vector3 startSize;
    private bool hasStartScene = true;

    private void Awake()
    {

        startSize = transform.localScale;
    }

    public void TweenShip()
    {
        if (hasStartScene == false)
        {
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(new Vector3(0f, 0f, 0f), 0.05f).OnComplete(() =>
            {
                gameObject.transform.DOShakePosition(0.40f, 0.08f, 40, 90f).SetDelay(0.8f).OnComplete(() => { gameObject.transform.position = new Vector3(0, 3, 0); } );
                gameObject.transform.DOScale(startSize, 0.2f).SetDelay(0.8f);

            });
        }
        hasStartScene = false;
    }

}
