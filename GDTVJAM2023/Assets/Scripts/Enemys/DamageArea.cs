using UnityEngine;
using DG.Tweening;


public class DamageArea : MonoBehaviour
{
    public SpriteRenderer spR;

    private void OnEnable()
    {
        // FadeIn
        spR.DOFade(0.03f, 4f);
        gameObject.transform.DOScale(new Vector3(15, 15, 15), 4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
    }

    public void FadeOut()
    {
        gameObject.transform.DOScale(new Vector3(0, 0, 0), 0.5f).OnComplete(() => { gameObject.SetActive(false); });
    }
}
