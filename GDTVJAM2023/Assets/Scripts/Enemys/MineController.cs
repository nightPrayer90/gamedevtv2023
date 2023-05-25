using UnityEngine;

public class MineController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float detectionRange = 10f;

    private Material[] materialList;
    private Material buildingMaterial;
    private Material emissivMaterial;
    private GameObject player;

    private EnemyHealth enemyHealth;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        player = GameObject.Find("Player");

        materialList = GetComponent<MeshRenderer>().materials;
        buildingMaterial = materialList[0];
        emissivMaterial = materialList[1];

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
            }
            else
            {
                materialList[1] = buildingMaterial;
                GetComponent<MeshRenderer>().materials = materialList;
            }
        }
    }
}