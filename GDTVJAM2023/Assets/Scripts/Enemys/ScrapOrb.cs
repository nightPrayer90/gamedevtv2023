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
        pickUp.gameManager.AddScrap(scrap);

        var floatingString = $" +{scrap} scrap";
        pickUp.gameManager.DoFloatingText(transform.position, floatingString, textColor);

        pickUp.OnCollect -= PickUp;
    }
}
