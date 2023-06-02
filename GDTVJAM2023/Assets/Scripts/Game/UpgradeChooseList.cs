using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChooseList : MonoBehaviour
{
    public List<int> upgradeIndex = new List<int>(); // Die Liste der Werte

    private void Start()
    {
        upgradeIndex.Add(0);
        upgradeIndex.Add(1);
        upgradeIndex.Add(2);
        upgradeIndex.Add(3);
        upgradeIndex.Add(4);
        upgradeIndex.Add(5);
    }

}
