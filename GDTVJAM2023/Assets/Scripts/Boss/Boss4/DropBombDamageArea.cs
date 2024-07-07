using UnityEngine;
using DG.Tweening;

public class DropBombDamageArea : MonoBehaviour
{
    public SpriteRenderer sprite;
    private void Start()
    {
        transform.DOScale(2.5f, 1f);
        sprite.DOFade(0.5f, 1f);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 6f, transform.position.z);
    }
    
    public void DestroyDamageArea()
    {
        transform.DOScale(0f, 0.5f);
        sprite.DOFade(0, 0.5f);
    }

    private void OnDestroy()
    {
        transform.DOKill();
        sprite.DOKill();
    }

}
