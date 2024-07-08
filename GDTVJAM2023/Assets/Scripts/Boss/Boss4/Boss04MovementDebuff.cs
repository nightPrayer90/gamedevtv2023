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

        if (playerController.isDebuff == false)
        {
            playerController.SetMoveControlDebuff();
            playerController.isDebuff = true;
            Invoke(nameof(DestroyObject), lifeTime);
        }
        else
        {
            Destroy();
        }
    }
        
    public void DestroyObject()
    {
        AudioManager.Instance.PlaySFX("DropBompPlayerDebuffEnd");
        playerController.SetMoveControlDebuff();
        playerController.isDebuff = false;
        gameObject.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f);
        Invoke(nameof(Destroy), 0.5f);
        debuffPS.Stop();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
