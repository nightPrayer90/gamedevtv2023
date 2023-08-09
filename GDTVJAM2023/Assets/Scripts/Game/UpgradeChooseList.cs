using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChooseList : MonoBehaviour
{
    public List<int> upgradeIndex = new List<int>(); // Die Liste der Werte
    public List<int> classUpgradeIndex = new List<int>(); // Die Liste der Werte

    private void Start()
    {
        upgradeIndex.Add(0);
        upgradeIndex.Add(1);
        upgradeIndex.Add(2);
        upgradeIndex.Add(3);
        upgradeIndex.Add(4);
        upgradeIndex.Add(5);

        classUpgradeIndex.Add(18);
        classUpgradeIndex.Add(19);
        classUpgradeIndex.Add(20);
        classUpgradeIndex.Add(21);
        classUpgradeIndex.Add(22);
        classUpgradeIndex.Add(23);
        classUpgradeIndex.Add(24);
        classUpgradeIndex.Add(25);
    }

}
