using DG.Tweening;
using UnityEngine;

public class Boss04MovementDebuff : MonoBehaviour
{
    public ParticleSystem debuffPS;
    private Rigidbody playerRB;
    private NewPlayerController playerController;

    private void Awake()
    {
        playerRB = GameObject.Find("NewPlayer").GetComponent<Rigidbody>();
        gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 2);
        playerController = playerRB.gameObject.GetComponent<NewPlayerController>();
    }

    public void ActivateEffect(float lifeTime)
    {
        if (playerController == null)
        {
            playerRB = GameObject.Find("NewPlayer").GetComponent<Rigidbody>();
            playerController = playerRB.gameObject.GetComponent<NewPlayerController>();
        }
        playerController.SetMoveControlDebuff();

        Invoke(nameof(DestroyObject), lifeTime);
    }
        
    public void DestroyObject()
    {
        playerController.SetMoveControlDebuff();
        gameObject.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f);
        Invoke(nameof(Destroy), 0.5f);
        debuffPS.Stop();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
