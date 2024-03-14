using UnityEngine;

public class FrontShieldSpawner : MonoBehaviour
{
    public GameObject shieldObject;
    private PlayerWeaponController playerWaeponController;
    private ShieldController enabledShieldController;
    private UpgradeChooseList UpgradeChooseList;
    public float spawnInterval = 10f;
    //public int shieldLife = 1;

    private void Start()
    {
        playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        UpgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();

        //time to the first spawn
        SpawnFrondShieldControl();    
    }

    public void SpawnFrondShieldControl()
    {
        float resultSpawnInterval = spawnInterval;
        if (UpgradeChooseList.weaponIndexInstalled[61] == 1)
            resultSpawnInterval *= 0.5f;

        Invoke("SpawnFrontShield", resultSpawnInterval);
    }

    public void SpawnFrontShield()
    {
        // FrontShield
        if (enabledShieldController == null)
        {
            GameObject enabledShieldObject = Instantiate(shieldObject, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation);
            playerWaeponController.frontShield_ = enabledShieldObject;
            enabledShieldController = enabledShieldObject.GetComponent<ShieldController>();
            enabledShieldController.ShieldEnable(playerWaeponController.fsShieldLife);
        }
        else
        {
            if (enabledShieldController != null)
                enabledShieldController.ShieldEnable(playerWaeponController.fsShieldLife);
        }
    }

}
