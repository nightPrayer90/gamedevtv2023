using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss07 : MonoBehaviour
{
    public GameObject[] objectsToActivate; // Array für die zu aktivierenden Gameobjekte
    public GameObject[] weapons;

    public float activationDelay = 0.5f; // Zeitverzögerung zwischen den Aktivierungen

    public MineController mineController;
    public GameObject explosionObject;

    private void Start()
    {
        StartCoroutine(ActivateObjectsWithDelay());
        mineController.detectionRange = 100;
        mineController.rotationSpeed = 35;
    }

    private IEnumerator ActivateObjectsWithDelay()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
            Instantiate(explosionObject, obj.transform.position, obj.transform.rotation);
            yield return new WaitForSeconds(activationDelay);
            
        }
        BattleStarts();

    }

    private void BattleStarts()
    {
        foreach (GameObject weapon in weapons)
        {
            if (weapon != null)
                weapon.SetActive(true);
        }
    }
 }