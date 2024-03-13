using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    [Header("Mine Settings")]
    public float rotationSpeed = 10f;
    public float detectionRange = 10f;
    public Material buildingMaterial;
    public Material emissivMaterial;
    private Material[] materialList;
    private bool isDetected = false;
    private bool isActive = false;

    // game Objects
    private GameObject player;
    private EnemyHealth enemyHealth;
    private GameManager gameManager;

    public List<ParticleSystem> mineWeapons;
    public AudioSource shootSound;

    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */

    private void Start()
    {
        // set components
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyHealth = gameObject.GetComponent<EnemyHealth>();
        player = GameObject.FindWithTag("Player");

        // Update Materials
        materialList = GetComponent<MeshRenderer>().materials;
        materialList[0] = buildingMaterial;
        materialList[1] = buildingMaterial;
        GetComponent<MeshRenderer>().materials = materialList;

        InvokeRepeating("DetectPlayer", 1f, 0.1f);


    }

    private void FixedUpdate()
    {
        if (isDetected == true)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);   
    }

    private void Shooting()
    {
        foreach (ParticleSystem weapon in mineWeapons)
        {
            weapon.Emit(1);
        }
        if (shootSound != null)
        {
            shootSound.volume = AudioManager.Instance.sfxVolume;
            shootSound.Play();
        }
    }

    // Invoke aus der Startmethode
    private void DetectPlayer()
    {
        isDetected = false;
        if (gameManager.dimensionShift == enemyHealth.secondDimensionEnemy)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
            {
                isDetected = true;
                if (isActive == false)
                    ToggleMaterial(true);
            }
        }

        if (isDetected == false && isActive == true)
            ToggleMaterial(false);
    }

    private void ToggleMaterial(bool toggleFlag)
    {
        if (toggleFlag == true)
        {
            materialList[1] = emissivMaterial;
            GetComponent<MeshRenderer>().materials = materialList;
            isActive = true;
            InvokeRepeating("Shooting", 0.5f, 1f);
            Debug.Log("StartSooting");
        }

        // deactivate Mine
        else
        {
            materialList[1] = buildingMaterial;
            GetComponent<MeshRenderer>().materials = materialList;
            isActive = false;
            CancelInvoke("Shooting");
        }

    }
}