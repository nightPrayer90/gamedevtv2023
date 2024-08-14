using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MineController : MonoBehaviour
{
    [Header("Mine Settings")]
    public bool isSecondDimensionEnemy = false;
    public float rotationSpeed = 10f;
    public float detectionRange = 10f;
    public float fireRate = 1f;

    public Material buildingMaterial;
    public Material emissivMaterial;
    private Material[] materialList = new Material[2] {null, null};
    public MeshRenderer meshRenderer;
    private bool isDetected = false;
    private bool isActive = false;

    // game Objects
    private Transform playerTr;
    private GameManager gameManager;
    
    public List<GameObject> mineWeapons;
    public List<ParticleSystem> mineWeaponParticels;
    private AudioSource shootSound;

    /* **************************************************************************** */
    /* LIFECYCLE AND EVENTS ------------------------------------------------------- */
    /* **************************************************************************** */
    #region LIFECYCLE AND EVENTS
    private void Awake()
    {
        shootSound = gameObject.GetComponent<AudioSource>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerTr = GameObject.FindWithTag("Player").transform;

        materialList[0] = buildingMaterial;
        materialList[1] = buildingMaterial;
    }

    private void OnEnable()
    {
        ToggleMaterial(false);
        InvokeRepeating("DetectPlayer", 1f, 0.1f);
    }

    private void FixedUpdate()
    {
        if (isActive == true)
        {
            RotateWeaponToPlayer2(0);
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        isActive = false;
        isDetected = false;
    }
    #endregion



    /* **************************************************************************** */
    /* DETECT --------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region DETECT
    // Invoke aus der Startmethode
    private void DetectPlayer()
    {
        isDetected = false;

        if (gameManager.dimensionShift == isSecondDimensionEnemy)
        {
            float distance = Vector3.Distance(transform.position, playerTr.position);

            if (distance <= detectionRange)
            {
                isDetected = true;
                if (isActive == false)
                    ToggleMaterial(true);
            }
        }

        if (isDetected == false && isActive == true)
            ToggleMaterial(false);
    }
    #endregion



    /* **************************************************************************** */
    /* ATTACK --------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region ATTACK
    private void Shooting()
    {
        foreach (ParticleSystem weapon in mineWeaponParticels)
        {
            weapon.Emit(1);
        }
        if (shootSound != null)
        {
            shootSound.volume = AudioManager.Instance.sfxVolume;
            shootSound.Play();
        }
    }


    // control the frontweapon
    private void RotateWeaponToPlayer2(int frontWeaponMaxCount_)
    {
        Vector3 directionToPlayer = playerTr.position - mineWeapons[frontWeaponMaxCount_].transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        Quaternion targetRotation = mineWeapons[0].transform.rotation;
        targetRotation = Quaternion.Slerp(targetRotation, lookRotation, 3 * Time.deltaTime);

        mineWeapons[frontWeaponMaxCount_].transform.rotation = targetRotation;
    }
    #endregion


    /* **************************************************************************** */
    /* MISC ----------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region MISC

    private void ToggleMaterial(bool toggleFlag)
    {
        // activate Mine
        if (toggleFlag == true)
        {
            materialList[1] = emissivMaterial;
            meshRenderer.materials = materialList;
            isActive = true;
            InvokeRepeating("Shooting", 0.5f, fireRate);
        }

        // deactivate Mine
        else
        {
            materialList[1] = buildingMaterial;
            meshRenderer.materials = materialList;
            isActive = false;
            CancelInvoke("Shooting");
        }

    }
    #endregion
}