using DG.Tweening;
using UnityEngine;

public class Boss04DamageZone : MonoBehaviour
{
    public bool trackToPlayer = true;
    public SpriteRenderer damageZone;
    private Rigidbody playerRB;

    private void Awake()
    {
        playerRB = GameObject.Find("NewPlayer").GetComponent<Rigidbody>();
        gameObject.transform.DOScale(new Vector3( 1f, 1f, 1f), 2);
        damageZone.DOFade(0.4f, 2f).SetEase(Ease.InCirc);
    }

    // Update is called once per frame
    void Update()
    {
        if (trackToPlayer == true)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, playerRB.position, 10f * Time.deltaTime);
            transform.position = newPosition;
        }
    }

    public void DestroyObject()
    {
        gameObject.transform.DOKill();
        damageZone.transform.DOKill();
        gameObject.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f);
        damageZone.DOFade(0.0f, 0.2f);
        Invoke(nameof(Destroy), 0.3f);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
