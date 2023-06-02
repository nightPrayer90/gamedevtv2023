using UnityEngine;

public class NavigationController : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnDistrictList spawnDoistrictList;
    public GameObject navigatorMesh;
    public Vector3 targetPosition;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnDoistrictList = GameObject.Find("Game Manager").GetComponent<SpawnDistrictList>();
        
    }

    public void SetTargetPosition()
    {
        // Position des GoToDimensionPickups
        targetPosition = spawnDoistrictList.goToDimensionPickup[gameManager.districtNumber - 1].transform.position;
        navigatorMesh.SetActive(true);
        AudioManager.Instance.PlaySFX("ShortAlert");
    }

    public void DeactivateNavigatorMesh()
    {
        navigatorMesh.SetActive(false);
    }
  
}