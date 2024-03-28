using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleValues : MonoBehaviour
{
    public string moduleName; // set from other objects (create)
    [TextArea]
    public string modulDescription_multiLineText;
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
