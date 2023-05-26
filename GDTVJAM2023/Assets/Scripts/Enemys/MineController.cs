using UnityEngine;

public class MineController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float detectionRange = 10f;

    private Material[] materialList;
    public Material buildingMaterial;
    public Material emissivMaterial;
    private GameObject player;

    private EnemyHealth enemyHealth;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        player = GameObject.Find("Player");

        materialList = GetComponent<MeshRenderer>().materials;
        materialList[0] = buildingMaterial;
        materialList[1] = buildingMaterial;
        GetComponent<MeshRenderer>().materials = materialList;
    }

    private void Update()
    {
        

        if (gameManager.dimensionShift == enemyHealth.secondDimensionEnemy)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
            {
                materialList[1] = emissivMaterial;
                GetComponent<MeshRenderer>().materials = materialList;
                enemyHealth.StartShooting();
            }
            else
            {
                materialList[1] = buildingMaterial;
                GetComponent<MeshRenderer>().materials = materialList;
                enemyHealth.StopShooting();
            }
        }
    }
}