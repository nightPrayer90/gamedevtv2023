using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChooseList : MonoBehaviour
{
    public List<int> weaponIndex = new List<int>(); // Die Liste der Werte

    private void Start()
    {
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
    }
}
