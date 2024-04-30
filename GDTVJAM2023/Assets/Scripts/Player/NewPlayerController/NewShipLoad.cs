using System.Collections.Generic;
using UnityEngine;

public class NewShipLoad : MonoBehaviour
{
    public List<ModuleDataRuntime> installedModuleData;
    public ModuleDataRuntime[,] installedModuleGrid;
    public ModuleList moduleList;
    public Transform transformParent;
    
    //PlayerData
    public PlayerData playerData;


    /* **************************************************************************** */
    /* LIFECYCLE------------------------------------------------------------------- */
    /* **************************************************************************** */

    void Start()
    {
        installedModuleData = new();
        foreach (ModuleData item in playerData.moduleData)
        {
            installedModuleData.Add(new ModuleDataRuntime(item));
        }

        BuildShipFromModuleData();
    }


    public void BuildShipFromModuleData()
    {
        foreach (ModuleDataRuntime instance in installedModuleData)
        {

            GameObject go = Instantiate(moduleList.moduls[instance.moduleTypeIndex].modulePrefabs, transformParent, false);
            go.transform.localPosition = new Vector3(instance.x, 0, instance.z);
            go.transform.localRotation = Quaternion.Euler(0, 0, 0);

            BaseModule[] baseModules = go.GetComponents<BaseModule>();

            int i = 0;
            //copy all Module Vales
            foreach (BaseModule bm in baseModules)
            {
                if (bm != null)
                {
                    if (i == 0)
                    {
                        bm.moduleValues = moduleList.moduls[instance.moduleTypeIndex].moduleValues;
                    }
                }
                i++;

            }
        }
    }

}
