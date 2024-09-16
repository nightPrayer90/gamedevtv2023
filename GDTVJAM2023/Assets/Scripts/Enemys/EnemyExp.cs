using Unity.VisualScripting;
using UnityEngine;

public class EnemyExp : MonoBehaviour
{
    public int expValue = 1;
    public PickUps pickUp;
    private NewPlayerController playerController;

    private void OnEnable()
    {
        pickUp.OnCollect += PickUp;
    }

    private void OnDisable()
    {
        pickUp.OnCollect -= PickUp;
    }

    private void PickUp()
    {
        playerController = pickUp.playerController;
        playerController.UpdatePlayerExperience(expValue);
    }
}


