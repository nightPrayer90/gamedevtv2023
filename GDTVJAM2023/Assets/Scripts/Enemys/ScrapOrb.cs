using UnityEngine;

public class ScrapOrb : MonoBehaviour
{
    public int scrap = 1;
    public PickUps pickUp;
    public Color textColor = new();

    private void OnEnable()
    {
        pickUp.OnCollect += PickUp;
    }

    private void PickUp()
    {
        AudioManager.Instance.PlaySFX("ExperienceOrb");

        pickUp.gameManager.AddScrap(scrap);

        var floatingString = $" +{scrap} Scraps";
        pickUp.gameManager.DoFloatingText(transform.position, floatingString, textColor);

        pickUp.OnCollect -= PickUp;
    }
}
