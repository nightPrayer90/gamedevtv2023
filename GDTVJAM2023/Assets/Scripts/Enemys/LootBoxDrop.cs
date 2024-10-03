
using UnityEngine;
using System;
using DG.Tweening.Core.Easing;

public class LootBoxDrop : MonoBehaviour
{
    [Serializable]
    public struct lootObject
    {
        public GameObject lootprefab;
        public int quanitiy;
    }

    private GameManager gameManager;
    private EnemyHealth enemyHealth;
    [SerializeField] private int lootCount = 6;
    [SerializeField] private lootObject[] lootObjects;

    private int maxValue = 0;
    public float explosionDelay = 0f;
    public bool isBoss = false;

    [Header("Extra Upgrades")]
    public GameObject bigHealthOrb;
    public GameObject bigEXPOrb;
    public GameObject bigTimerOrb;


    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        maxValue = 0;
        foreach (lootObject lootObject in lootObjects)
        {
            maxValue += lootObject.quanitiy;
        }
    }

    private void OnEnable()
    {
        enemyHealth.DieEvent += OnDie;
        gameManager.lootboxContainer++;
    }

    private void OnDisable()
    {
        enemyHealth.DieEvent -= OnDie;
        gameManager.lootboxContainer--;
    }

    private void OnDie(object sender, EventArgs e)
    {
        Invoke(nameof(ExplosionWithDelay), explosionDelay);
    }

    private void ExplosionWithDelay()
    {
        // box spawn 1 Magnet or some loot like lootCount;
        if (UnityEngine.Random.Range(0, maxValue - 1) <= lootObjects[0].quanitiy)
        {
            // spawn Magnet
            ObjectPoolManager.SpawnObject(lootObjects[0].lootprefab, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        else
        {
            var extraLoot = 0;
            if (isBoss == false)
            {
                SpawnAdditionalObjects();
                extraLoot = gameManager.upgradeChooseList.upgrades[91].upgradeIndexInstalled * 2; // Orb Jackpot
            }

            // spawn some other Stuff
            for (int i = 0; i < lootCount + extraLoot; i++)
            {
                SpawnObjects();
            }

            
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    private void SpawnAdditionalObjects()
    {
        if (gameManager.upgradeChooseList.upgrades[88].upgradeIndexInstalled > 0 && bigHealthOrb != null) // Life Orb Boost
        {
            SpawnLoot(bigHealthOrb);
        }

        if (gameManager.upgradeChooseList.upgrades[89].upgradeIndexInstalled > 0 && bigEXPOrb != null) // EXP Orb Boost
        {
            for (int i = 0; i < 2; i++)
            {
                SpawnLoot(bigEXPOrb);
            }
        }

        if (gameManager.upgradeChooseList.upgrades[90].upgradeIndexInstalled > 0 && bigTimerOrb != null) // Time Orb Boost
        {
            SpawnLoot(bigTimerOrb);
        }
    }

    private void SpawnObjects()
    {
        int ran = lootObjects[0].quanitiy + UnityEngine.Random.Range(1, maxValue - lootObjects[0].quanitiy);
        int value = 0;

        foreach (lootObject lootObject in lootObjects)
        {
            value += lootObject.quanitiy;
            if (ran <= value)
            {
                SpawnLoot(lootObject.lootprefab);
                break;
            }
        }
    }

    private void SpawnLoot(GameObject lootPrefab)
    {
        GameObject go = ObjectPoolManager.SpawnObject(lootPrefab, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);
        // Überprüfen, ob das Objekt ein Rigidbody hat, ansonsten hinzufügen
        Rigidbody rb = go.GetComponent<Rigidbody>();

        // Erstellen eines zufälligen Vektors für die Richtung des Impulses
        float randomX = UnityEngine.Random.Range(-1f, 1f);
        float randomZ = UnityEngine.Random.Range(-1f, 1f);
        Vector3 randomDirection = new Vector3(randomX, 0, randomZ).normalized; // Normalisiert, um gleiche Stärke in alle Richtungen zu haben

        // Festgelegte Y-Achse (6) und Skalierung der Kraft
        Vector3 force = new Vector3(randomDirection.x, 6, randomDirection.z) * UnityEngine.Random.Range(0.08f, 0.3f); // Skalierung des Impulses

        // Anwenden des Impulses auf das Rigidbody-Objekt
        rb.AddForce(force, ForceMode.Impulse);
    }
}
