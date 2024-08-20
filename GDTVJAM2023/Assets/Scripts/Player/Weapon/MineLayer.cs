using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLayer : MonoBehaviour
{
    [Header("Settings")]
    public int baseDamage = 5;
    public float lifeTime = 3f;
    public float spawnInterval = 1f;
    public GameObject MineToLaunch;
    public GameObject spawnPoint;
    private UpgradeChooseList upgradeChooseList;
    private bool isMainWeapon = false;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();
        StartValues();
    }




    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // set start values fom the weaponController
    public void StartValues()
    {
        PlayerWeaponController weaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        baseDamage = weaponController.mlDamage;
        lifeTime = weaponController.mlLifetime;
        spawnInterval = weaponController.mlReloadTime;
        UpdateInvoke();
    }

    public void UpdateInvoke()
    {
        CancelInvoke();
        InvokeRepeating("SpawnMine", spawnInterval, spawnInterval);
    }


    // Invoke, spawns a rocked after spawnInterval sec
    private void SpawnMine()
    {
        GameObject go = ObjectPoolManager.SpawnObject(MineToLaunch, spawnPoint.transform.position, Quaternion.Euler(0f, 180f, 0f) * gameObject.transform.rotation, ObjectPoolManager.PoolType.Gameobject);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        PlayerMineController mine = go.GetComponent<PlayerMineController>();
        mine.damage = baseDamage;
        mine.isMainWeapon = false;

        AudioManager.Instance.PlaySFX("PlayerFireFlies");
        if (rb != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle;
            Vector3 direction = new Vector3(randomDirection.x, 0f, randomDirection.y);
            rb.AddForce(direction * 0.3f, ForceMode.Impulse);
        }
    }

}


