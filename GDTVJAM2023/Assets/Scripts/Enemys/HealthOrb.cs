using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    public int health = 1;
    public PickUps pickUp;
    public Color textColor = new();
    private NewPlayerController playerController;

    private void OnEnable()
    {
        pickUp.OnCollect += PickUp;
    }

    private void PickUp()
    {
        playerController = pickUp.playerController;
        playerController.UpdatePlayerHealth(-health);

        var floatingString = $" +{health} hp";
        pickUp.gameManager.DoFloatingText(transform.position, floatingString, textColor);

        AudioManager.Instance.PlaySFX("ExperienceOrb");

        pickUp.OnCollect -= PickUp;
    }
}
