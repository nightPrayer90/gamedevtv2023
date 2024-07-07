using UnityEngine;
using DG.Tweening;

public class DropBombController : MonoBehaviour
{
    public DropBombDamageArea damageArea;
    public ParticleSystem bodyPS;
    private bool statusFlag = false;
    public GameObject debuffPrefab;
    public Collider sphereCollider;
    private bool activateFlag = false;
    public float debuffTime = 10f;

    private void Update()
    {
        if (transform.position.y < 6.4f && statusFlag == false)
        {
            statusFlag = true;
            damageArea.DestroyDamageArea();
            
            Invoke(nameof(Explode), 1f);
            bodyPS.transform.DOScale(0.8f, 0.3f).OnComplete(() =>
            {
                bodyPS.Stop();
                bodyPS.transform.DOScale(0.0f, 0.2f);
            });
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && activateFlag == false)
        {
            GameObject go = Instantiate(debuffPrefab, other.transform.position, transform.rotation,other.transform);
            Boss04MovementDebuff MD = go.GetComponent<Boss04MovementDebuff>();
            MD.ActivateEffect(debuffTime);
            sphereCollider.enabled = false;
            activateFlag = true;
        }
    }

    private void Explode()
    {
        Destroy(gameObject);
    }
}
