using UnityEngine;
using DG.Tweening;


public class DamageArea : MonoBehaviour
{
    public SpriteRenderer spR;
    public ParticleSystem damageParticle;
    public ParticleSystem waveParticle;

    private void OnEnable()
    {
        // FadeIn
        damageParticle.Play();
        spR.DOFade(0.2f, 4f);
        gameObject.transform.DOScale(new Vector3(2.2f, 2.2f, 2.2f), 4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
    }

    public void FadeOut()
    {
        damageParticle.Stop();
        waveParticle.Play();
        gameObject.transform.DOScale(new Vector3(0, 0, 0), 0.6f).OnComplete(() => { gameObject.SetActive(false); });
        
    }
}
