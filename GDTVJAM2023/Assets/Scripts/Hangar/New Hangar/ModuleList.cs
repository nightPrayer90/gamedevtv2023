using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Modules
{
    public enum ModulTyp
    {
        Connector,
        MainEngine,
        StrafeEngine,
        DirectionEngine,
        Cockpit,
        Weapon
    }

    public string moduleName;
    public GameObject modulePrefabs;
    public GameObject hangarPrefab;
    public Mesh modulMesh;
    public Sprite modulSprite;
    public ModulTyp myEnumVariable;
    public bool canLeft = false;
    public bool canRight = false;
    public bool canFront = false;
    public bool canBack = false;

}


[CreateAssetMenu(fileName = "ModuleList", menuName = "Scriptable Objects/ModuleList")]
public class ModuleList : ScriptableObject
{
    public List<Modules> moduls;
}
