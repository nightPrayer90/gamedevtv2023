using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    [System.Serializable]
    public struct AllyList
    {
        public string allyName;
        public int allyIndex;
        public string allyDiscription;
        public Sprite allySprite;
        public GameObject AllyPrefab;
    }

    public List<AllyList> allylist = new();
    public int[] spawnDistricts = new int[9];


    private GameManager gameManager;

    private void Awake()
    {
        gameManager = gameObject.GetComponentInParent<GameManager>();
        BuildAllyDistrictList();
    }

    private void BuildAllyDistrictList()
    {
        List<int> tempAllyList = new();
        int allyDy1 = Random.Range(0, 2);
        int allyDy2 = Random.Range(3, 5);
        int allyDy3 = Random.Range(6, 7);

        for (int i = 0; i < allylist.Count; i++)
        {
            tempAllyList.Add(i+1);
        }

        for (int i = 0; i < 8; i++)
        {
            spawnDistricts[i] = 0;

            if (i == allyDy1)
            {
                int randomListIndex = Random.Range(0, tempAllyList.Count);

                spawnDistricts[i] = tempAllyList[randomListIndex];
                tempAllyList.RemoveAt(randomListIndex);
            }
            else if (i == allyDy2)
            {
                int randomListIndex = Random.Range(0, tempAllyList.Count);

                spawnDistricts[i] = tempAllyList[randomListIndex];
                tempAllyList.RemoveAt(randomListIndex);
            }
            else if (i == allyDy3)
            {
                int randomListIndex = Random.Range(0, tempAllyList.Count);

                spawnDistricts[i] = tempAllyList[randomListIndex];
                tempAllyList.RemoveAt(randomListIndex);
            }
        }
    }
}
