using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss03_ : MonoBehaviour
{
    public GameObject[] objectsToActivate; // Array für die zu aktivierenden Gameobjekte
    public GameObject[] weapons;

    public float activationDelay = 0.5f; // Zeitverzögerung zwischen den Aktivierungen

    public MineController mineController;
    public GameObject explosionObject;

    public GameObject shootRotation;
    public float shootInterval = 1f;
    public float setRotationAngle = 45f;
    private float rotationAngle = 0f;

    private void Start()
    {
        StartCoroutine(ActivateObjectsWithDelay());
        
        mineController.detectionRange = 100;
        mineController.rotationSpeed = 10;

        

    }
    private IEnumerator ShootRotation()
    {
        while (true)
        {
            if (shootRotation.activeSelf)
            {
                shootRotation.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                rotationAngle = rotationAngle + setRotationAngle;
                Debug.Log("setRotation " + rotationAngle);
            }
            else
            {
                Destroy(gameObject);
                StopAllCoroutines();
            }
            yield return new WaitForSeconds(shootInterval);
        }

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

        StartCoroutine(ShootRotation());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}