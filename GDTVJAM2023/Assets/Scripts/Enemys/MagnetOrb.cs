using UnityEngine;

public class MagnetOrb : MonoBehaviour
{
    public PickUps pickUp;

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
        AudioManager.Instance.PlaySFX("ExperienceOrb");

        GameObject[] expObjects = GameObject.FindGameObjectsWithTag("Exp");

        foreach (GameObject obj in expObjects)
        {
            if (obj.activeInHierarchy)
            {
                obj.GetComponent<PickUps>().SetCollect();
            }
        }
    }
}
