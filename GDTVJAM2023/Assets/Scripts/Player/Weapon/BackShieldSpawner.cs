using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackShieldSpawner : MonoBehaviour
{
    public GameObject leftShieldObject;
    public GameObject rightShieldObject;
    private ShieldController leftShieldController;
    private ShieldController rightShieldContoller;

    private PlayerWeaponController playerWaeponController;

    public float spawnInterval = 10f;
    //public int shieldLife;
    private float randomSpawntime;

    private void Start()
    {
        playerWaeponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();

        //time to the first spawn
        randomSpawntime = Random.Range(0.2f, 0.4f);

        SpawnBackShildRightControl();
        SpawnBackShildLeftControl();
    }

    public void SpawnBackShildLeftControl()
    {
        Invoke("SpawnLeftBackShield", spawnInterval + randomSpawntime);
    }
    public void SpawnBackShildRightControl()
    {
        Invoke("SpawnRightBackShield", spawnInterval - randomSpawntime);
    }

    public void SpawnLeftBackShield()
    {
        // FrontShield
        if (leftShieldController == null)
        {
            GameObject enabledShieldObject = Instantiate(leftShieldObject, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation);
            playerWaeponController.backShieldLeft_ = enabledShieldObject;
            leftShieldController = enabledShieldObject.GetComponent<ShieldController>();
            leftShieldController.ShieldEnable(playerWaeponController.bsShildLife);
        }
        else
        {
            if (leftShieldController != null)
            {
                leftShieldController.ShieldEnable(playerWaeponController.bsShildLife);
            }
        }
    }

    public void SpawnRightBackShield()
    {
        // FrontShield
        if (rightShieldContoller == null)
        {
            GameObject enabledShieldObject = Instantiate(rightShieldObject, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f) * gameObject.transform.rotation);
            playerWaeponController.backShieldRight_ = enabledShieldObject;
            rightShieldContoller = enabledShieldObject.GetComponent<ShieldController>();
            rightShieldContoller.ShieldEnable(playerWaeponController.bsShildLife);
            randomSpawntime = 0;
        }
        else
        {
            if (rightShieldContoller != null)
            {
                rightShieldContoller.ShieldEnable(playerWaeponController.bsShildLife);
            }
        }
    }
}
