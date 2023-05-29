using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss03 : MonoBehaviour
{
    public GameObject[] objectsToActivate; // Array für die zu aktivierenden Gameobjekte
    public GameObject[] weapons;

    public float activationDelay = 0.5f; // Zeitverzögerung zwischen den Aktivierungen

    public MineController mineController;
    public GameObject explosionObject;

    public ParticleSystem particleSystem1;
    public float particleSystem1Duration = 5f;

    public ParticleSystem particleSystem2;
    public float particleSystem2Duration = 3f;

    private float particleSystem1Timer;
    private float particleSystem2Timer;


    private void Start()
    {
        //StartCoroutine(ActivateObjectsWithDelay());
        mineController.detectionRange = 100;
        mineController.rotationSpeed = 35;

        particleSystem1.Stop();
        particleSystem2.Stop();
        particleSystem1Timer = particleSystem1Duration;
        particleSystem2Timer = particleSystem2Duration;
    }

    private void Update()
    {
        if (particleSystem1Timer > 0f)
        {
            particleSystem1Timer -= Time.deltaTime;

            if (!particleSystem1.isPlaying)
            {
                particleSystem1.Play();
            }
         
        }
        else
        {
            particleSystem1Timer = particleSystem1Duration;
            particleSystem1.Stop();
        }

        if (particleSystem2Timer > 0f)
        {
            particleSystem2Timer -= Time.deltaTime;

            if (!particleSystem2.isPlaying)
            {
                particleSystem2.Play();
            }
           
        }
        else
        {
            particleSystem2Timer = particleSystem2Duration;
            particleSystem2.Stop();
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
    }

    private void OnDestroy()
    {
        foreach (GameObject objects in objectsToActivate)
        {
            if (objects != null)
                objects.SetActive(true);
        }
    }
}