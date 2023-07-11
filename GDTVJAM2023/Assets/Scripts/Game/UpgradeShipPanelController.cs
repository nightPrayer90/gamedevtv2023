using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpgradeShipPanelController : MonoBehaviour
{
    private Vector3 localPositon;

    private void Awake()
    {
        localPositon = gameObject.transform.position;
    }
    private void OnEnable()
    {
        transform.position = localPositon;
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.6f, 8, 1).SetUpdate(true);
    }

    public void FadeOut()
    {
        transform.DOLocalMoveY(-600, 0.5f, true).SetUpdate(true).SetEase(Ease.InQuart);
    }

}
