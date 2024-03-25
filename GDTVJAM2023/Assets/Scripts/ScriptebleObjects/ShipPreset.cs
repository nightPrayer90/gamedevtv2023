using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShipPreset", menuName = "Scriptable Objects/ShipPreset")]
public class ShipPreset : ScriptableObject
{
    // Class Module Storage
    public List<ModuleData> baseModules;
}
