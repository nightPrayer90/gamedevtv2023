using System;
using UnityEngine;

[Serializable]
public class ModuleValues
{
    [HideInInspector] public string moduleName; // set from other objects (create)
    [HideInInspector] public ModuleType moduleType;
    [HideInInspector] public bool canLeft = false;
    [HideInInspector] public bool canRight = false;
    [HideInInspector] public bool canFront = false;
    [HideInInspector] public bool canBack = false;
    [Header("Hangar Only")]
    [TextArea(3,5)]
    public string modulDescription_multiLineText;

    [Header("Hangar and GamePlay")]
    public float costMass;
    public float costEnergie;
    public float energieProduction;
    public int energieStorage;
    public int health;
    public float protection;
    public float mainEngine;
    public float directionEngine;
    public float strafeEngine;
    public float boostEngine;
    public float boostStrafeEngine;
    public int bulletClass;
    public int rocketClass;
    public int laserClass;
    public int supportClass;
}
