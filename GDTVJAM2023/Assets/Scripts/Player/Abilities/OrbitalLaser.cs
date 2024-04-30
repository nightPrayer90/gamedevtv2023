using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalLaser : MonoBehaviour
{
    [HideInInspector] public float rotationSpeed = 10f;
    [HideInInspector] public int damage;
    public float realoadTime = 3f;
    public GameObject orbPrefab;
    [HideInInspector] public int orbCount = 3;
    public float orbRadius = 10f;
    private List<OrbitalLaserOrb> orbitalLaserOrbs = new();

    private Transform playerTransform;
    private PlayerWeaponController playerWeaponController;
    private UpgradeChooseList upgradeChooseList;


    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;

        playerWeaponController = player.GetComponent<PlayerWeaponController>();

        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();

        Invoke("SpawnNewOrbs", 1f);
    }

    public void SpawnNewOrbs()
    {
        DeleteOrbs();
        SpawnOrbs();
    }


    private void DeleteOrbs()
    {
        foreach (OrbitalLaserOrb orb in orbitalLaserOrbs)
        {
            Destroy(orb.gameObject);
        }
    }

    private void SpawnOrbs()
    {
        orbitalLaserOrbs.Clear();

        for (int i = 0; i < orbCount; i++)
        {
            float angle = i * (360f / orbCount); // Berechne den Winkel zwischen jedem Orb
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 spawnPosition = transform.position + rotation * Vector3.forward * orbRadius;

            // Erzeuge den Orb an den berechneten Koordinaten
            GameObject orb = Instantiate(orbPrefab, spawnPosition, Quaternion.identity);
            orb.transform.parent = gameObject.transform;

            OrbitalLaserOrb orbLaser = orb.GetComponentInChildren<OrbitalLaserOrb>();
            orbLaser.index = i + 1;
            orbLaser.damage = playerWeaponController.olDamage; //Debug - zeile hängt das spiel auf //Mathf.CeilToInt((float)playerWeaponController.olDamage * (1 + ((float)upgradeChooseList.percLaserDamage / 100)));
            orbLaser.realoadTime = realoadTime;
            orbitalLaserOrbs.Add(orbLaser);
        }
    }

    private void Update()
    {
        transform.position = playerTransform.position;
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    public void UpdateOrbs()
    {
        if (playerWeaponController != null)
        {

            if (orbitalLaserOrbs.Count < orbCount)
            {
                SpawnNewOrbs();
                return;
            }

            foreach (OrbitalLaserOrb orb in orbitalLaserOrbs)
            {
                orb.damage = Mathf.CeilToInt((float)playerWeaponController.olDamage * (1 + ((float)playerWeaponController.shipData.percLaserDamage / 100)));
            }
        }
    }
}
