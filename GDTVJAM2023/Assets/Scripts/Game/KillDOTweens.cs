using DG.Tweening;
using UnityEngine;

public class KillDOTweens : MonoBehaviour
{
    private void Awake()
    {
        DOTween.KillAll();
    }
}
