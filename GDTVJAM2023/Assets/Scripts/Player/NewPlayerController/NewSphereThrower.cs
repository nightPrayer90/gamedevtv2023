using UnityEngine;

public class NewSphereThrower : BaseModule
{
    [Header("Sphere Settings")]
    //public int baseDamage = 5;
    //public int baseResultDamage = 0;
    public float spawnInterval = 1f;
    public float spawnQuantity = 2f;
    public float forcePower = 0.4f;
    public float lifeTime = 10f;

    public GameObject spawnPoint;
    //public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);

    //private Objects
    public GameObject sphereToLaunch;
    private UpgradeChooseList upgradeChooseList;
    private PlayerWeaponController playerWeaponController;
    private NewPlayerController playerController;



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region Lifecycle Methoden
    void Start()
    {
        // Update Module Values to Player Controller - Function comes from BaseModule
        UpdateModuleValues();

        GameManager gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.gameObject.GetComponent<UpgradeChooseList>();
        playerController = GetComponentInParent<NewPlayerController>();
        playerController.OnIntroOver += HandleStartShooting;
        playerWeaponController = GetComponentInParent<PlayerWeaponController>();
        playerWeaponController.OnMWDamage += HandleDamageUpdate;

        UpdateInvoke();
        //HandleDamageUpdate(0);
    }

    public void HandleDamageUpdate(int damageToUpdate)
    {
        //baseDamage += damageToUpdate;
        //baseResultDamage = Mathf.CeilToInt((float)rockedBaseDamage * (1 + (upgradeChooseList.percRocketDamage / 100)));
    }

    private void HandleStartShooting()
    {
        UpdateInvoke();
    }
    #endregion



    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController


    public void UpdateInvoke()
    {
        CancelInvoke();
        InvokeRepeating("SpawnSphere", spawnInterval, spawnInterval);
    }


    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnSphere()
    {
        GameObject go = null;
        Rigidbody rb = null;
        SphereController sphere = null;

        for (int i = 1; i <= (spawnQuantity + upgradeChooseList.upgrades[82].upgradeIndexInstalled); i++)
        {
            int random = Random.Range(0, 5 + (i * 10));
            if (random < 15)
            {
                go = ObjectPoolManager.SpawnObject(sphereToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
                rb = go.GetComponent<Rigidbody>();
                sphere = go.GetComponent<SphereController>();
                sphere.SetDestroyTimer(lifeTime);

                if (rb != null)
                {
                    Vector3 randomDirection = Random.insideUnitCircle.normalized;
                    rb.AddForce(randomDirection * 0.5f, ForceMode.Impulse);
                }
            }
        }

        AudioManager.Instance.PlaySFX("PlayerFireFlies");
        
    }

}
