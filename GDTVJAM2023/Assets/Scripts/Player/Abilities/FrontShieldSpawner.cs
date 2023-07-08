using UnityEngine;

public class FrontShieldSpawner : MonoBehaviour
{
    public GameObject shieldObject;
    private PlayerWeaponController playerWaeponController;
    private ShieldController enabledShieldController;
    public float spawnInterval = 10f;
    public int shieldLife;

    

    private void Start()
    {
        playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        
        //time to the first spawn
        SpawnFrondShieldControl();    
    }

    public void SpawnFrondShieldControl()
    {
        Invoke("SpawnFrontShield", spawnInterval);
    }

    public void SpawnFrontShield()
    {
        // FrontShield
        if (enabledShieldController == null)
        {
            GameObject enabledShieldObject = Instantiate(shieldObject, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation);
            enabledShieldController = enabledShieldObject.GetComponent<ShieldController>();
        }
        else
        {
            if (enabledShieldController != null)
                enabledShieldController.ShieldEnable();
        }
    }

}
