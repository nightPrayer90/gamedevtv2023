using UnityEngine;

public class EnemyGroundDie : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
  
    private void OnEnable()
    {
        enemyHealth.gameManager.districtGroundEnemyControls[enemyHealth.gameManager.districtNumber-1] += 1;
    }

    private void OnDisable()
    {
        enemyHealth.gameManager.districtGroundEnemyControls[enemyHealth.gameManager.districtNumber-1] -= 1;
    }
  
}
