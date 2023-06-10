using UnityEngine;

public class MineController : MonoBehaviour
{
    [Header("Mine Settings")]
    public float rotationSpeed = 10f;
    public float detectionRange = 10f;
    public Material buildingMaterial;
    public Material emissivMaterial;
    private Material[] materialList;


    // game Objects
    private GameObject player;
    private EnemyHealth enemyHealth;
    private GameManager gameManager;




    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */

    private void Start()
    {
        // set coponents
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        player = GameObject.Find("Player");

        // Update Materials
        materialList = GetComponent<MeshRenderer>().materials;
        materialList[0] = buildingMaterial;
        materialList[1] = buildingMaterial;
        GetComponent<MeshRenderer>().materials = materialList;
    }

    private void Update()
    {
        
        if (gameManager.dimensionShift == enemyHealth.secondDimensionEnemy)
        {
            // rotation 
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            // set mine to aktiv
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
            {
                materialList[1] = emissivMaterial;
                GetComponent<MeshRenderer>().materials = materialList;
                enemyHealth.StartShooting();
            }

            // deactivate Mine
            else
            {
                materialList[1] = buildingMaterial;
                GetComponent<MeshRenderer>().materials = materialList;
                enemyHealth.StopShooting();
            }
        }
    }
}