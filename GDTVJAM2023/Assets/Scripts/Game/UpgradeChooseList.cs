using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChooseList : MonoBehaviour
{
    public List<int> upgradeIndex = new List<int>(); // normal upgrades
    public List<int> classUpgradeIndex = new List<int>(); // class upgrades
    public List<int> weaponIndex = new List<int>(); // weapon upgrades

    private void Start()
    {
        upgradeIndex.Add(0);
        upgradeIndex.Add(1);
        upgradeIndex.Add(2);
        upgradeIndex.Add(3);
        upgradeIndex.Add(4);
        upgradeIndex.Add(5);

        weaponIndex.Add(6);
        weaponIndex.Add(7);
        weaponIndex.Add(8);
        weaponIndex.Add(9);
        weaponIndex.Add(10);
        weaponIndex.Add(11);
        weaponIndex.Add(12);
        weaponIndex.Add(13);
        weaponIndex.Add(14);
        weaponIndex.Add(15);
        weaponIndex.Add(16);
        weaponIndex.Add(17);

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
