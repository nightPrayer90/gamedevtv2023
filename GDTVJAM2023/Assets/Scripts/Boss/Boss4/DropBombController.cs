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
    public AudioSource spawnSound;
    public AudioSource dieSound;


    private void Start()
    {
        spawnSound.volume = AudioManager.Instance.sfxVolume*0.3f;
        spawnSound.Play();
    }

    private void Update()
    {
        if (transform.position.y < 6.4f && statusFlag == false)
        {
            statusFlag = true;
            damageArea.DestroyDamageArea();
            dieSound.volume = AudioManager.Instance.sfxVolume + Random.Range(-0.2f, 0f); ;
            dieSound.pitch = Random.Range(0.8f, 1.2f);
            dieSound.Play();

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
            AudioManager.Instance.PlaySFX("DropBompPlayerHit");
        }
    }

    private void Explode()
    {
        Destroy(gameObject);
    }
}
