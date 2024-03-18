using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModuleList", menuName = "Scriptable Objects/ModuleList")]
public class ModuleList : ScriptableObject
{
    public List<GameObject> modulePrefabs;
}
