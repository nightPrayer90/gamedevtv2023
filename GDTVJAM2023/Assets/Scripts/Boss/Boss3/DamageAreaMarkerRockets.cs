using UnityEngine;
using DG.Tweening;


public class DamageAreaMarkerRockets : MonoBehaviour
{
    public SpriteRenderer spR;
    private Vector3 rocketTransform;
    public DropRocketController rocketController;

    private void OnEnable()
    {
        // FadeIn
        float diameter = rocketController.explosionRadius * 2;
        spR.DOFade(0.1f, .1f);
        gameObject.transform.DOScale(new Vector3(diameter, diameter, diameter), 0.1f);
        rocketTransform = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = rocketTransform;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void FadeOut()
    {
        gameObject.transform.DOScale(new Vector3(0, 0, 0), 0.2f).OnComplete(() => { gameObject.SetActive(false); });
    }
}
