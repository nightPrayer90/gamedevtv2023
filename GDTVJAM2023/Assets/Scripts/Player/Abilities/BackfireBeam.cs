using UnityEngine;
using UnityEngine.UIElements;


public class BackfireBeam : MonoBehaviour
{
    public GameObject BackfireBeamPrefab;
    private GameManager gameManager;
    //public float detectionRange = 10;
    //private GameObject nearestEnemy = null;
    //public int type = 0;
    private int novaTriggerCounter = 0;
    public int projectileCount = 1;
    public float realoadTime = 3f;
    public int damage = 4;
    public int killProjectileCount = 2;
    private UpgradeChooseList upgradeChooseList;

    private void Start()
    {
        StartFire(realoadTime);
        upgradeChooseList = GameObject.Find("Game Manager").GetComponent<UpgradeChooseList>();
    }

    public void StartFire(float reloadTime_)
    {
        // + Update Realoadtime
        StopFire();
        InvokeRepeating(nameof(ShootPrefab), 1f, reloadTime_);
    }

    public void StopFire()
    {
        CancelInvoke(nameof(ShootPrefab));
    }

    private void ShootPrefab()
    {
        if (novaTriggerCounter >= 100)
        {
            int projectileCount = 10;
            for (int i = 0; i < projectileCount; i++)
            {
                ObjectPoolManager.SpawnObject(BackfireBeamPrefab, transform.position, Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - ((360 / projectileCount) * i), transform.eulerAngles.z)), ObjectPoolManager.PoolType.Gameobject);
            }

            novaTriggerCounter = 0;
        }
        else
        {
            // normal attack
            /* switch (type)
             {
                 case 0: // after update
                     ObjectPoolManager.SpawnObject(BackfireBeamPrefab, transform.position, transform.rotation, ObjectPoolManager.PoolType.Gameobject);
                     break;
                 case 1: // homeing
                     HomeingShoot();
                     break;
                 case 2: // Mulitshoot
                     Mulitshoot(5);
                     break;
             }*/
            Mulitshoot(projectileCount);
        }
    }

    private void Mulitshoot(int length)
    {
        float anglePerStep = 20;
        float startingAngle = (anglePerStep * (length - 1)) / 2;

        for (int i = 0; i < length; i++)
        {
            ObjectPoolManager.SpawnObject(BackfireBeamPrefab, transform.position, Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - startingAngle + (anglePerStep * i), transform.eulerAngles.z)), ObjectPoolManager.PoolType.Gameobject);
        }

    }

    public void NovaTrigger()
    {
        if (upgradeChooseList.upgrades[85].upgradeIndexInstalled > 0)
            novaTriggerCounter++;
    }



    /*private void HomeingShoot()
    {
        // Homeing 
        FindNextTarget();
        if (nearestEnemy == null)
        {
            ObjectPoolManager.SpawnObject(BackfireBeamPrefab, transform.position, transform.rotation, ObjectPoolManager.PoolType.Gameobject);
        }
        else
        {
            // Homeing 
            Vector3 direction = (transform.position - nearestEnemy.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            ObjectPoolManager.SpawnObject(BackfireBeamPrefab, transform.position, lookRotation, ObjectPoolManager.PoolType.Gameobject);
        }
    }*/



    /*private void FindNextTarget()
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tagStr);

        float closestDistance = Mathf.Infinity;
        

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position); // Distanz zum Spielerobjekt berechnen

            // save the nearstEnemy
            if (distance <= detectionRange)
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }
    }*/
}
