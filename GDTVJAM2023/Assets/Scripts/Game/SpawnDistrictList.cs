using System.Collections.Generic;
using UnityEngine;

public class SpawnDistrictList : MonoBehaviour
{
    public List<GameObject> spawnManagerList = new List<GameObject>(); // Die Liste der Werte
    public List<int> waveKillList = new List<int>();
    public List<GameObject> districtList = new List<GameObject>();
    public List<GameObject> goToDimensionPickup = new List<GameObject>();


    public void Start()
    {
        bool isFirstPosition = true;

        foreach (GameObject district in districtList)
        {
            if (isFirstPosition)
            {
                isFirstPosition = false;
                continue; // ‹berspringe die erste Position
            }
            district.transform.position = district.transform.position + new Vector3 (0f,-6.5f,0f);
        }

        foreach (GameObject pickup in goToDimensionPickup)
        {
            pickup.SetActive(false);
        }
    }

}
