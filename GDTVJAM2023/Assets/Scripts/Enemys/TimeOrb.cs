using UnityEngine;

public class TimeOrb : MonoBehaviour
{
    public float time = 30;
    public PickUps pickUp;
    public Color textColor = new();


    private void OnEnable()
    {
        pickUp.OnCollect += PickUp;
    }

    private void PickUp()
    { 
        AudioManager.Instance.PlaySFX("ExperienceOrb");

        pickUp. gameManager.UpdateTimer(time);

        var floatingString = $" +{time} sec";
        pickUp.gameManager.DoFloatingText(transform.position, floatingString, textColor);

        pickUp.OnCollect -= PickUp;
    }
}
