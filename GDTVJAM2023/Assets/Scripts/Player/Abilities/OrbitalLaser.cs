using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalLaser : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 10f;
    public int damage;
    public float realoadTime;
    private GameObject player;
    private Transform playerTransform;
    private PlayerWeaponController playerWeaponController;
    public List<OrbitalLaserOrb> orbitalLaserOrbs;
    

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerWeaponController = player.GetComponent<PlayerWeaponController>();
        playerTransform = player.transform;
        UpdateOrbs();
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
            foreach (OrbitalLaserOrb orb in orbitalLaserOrbs)
            {
                orb.damage = playerWeaponController.olDamage;
                orb.realoadTime = playerWeaponController.olReloadTime;
            }
        }
    }
}
