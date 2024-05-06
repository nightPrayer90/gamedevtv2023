using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Definition Control")]
    public bool canPoolObject = true;
    public bool isGroundUnit = false;
    public bool secondDimensionEnemy = false;
    public bool isOutOfDimension = false;//
    //public bool isdied = false; // TODO: Check it
    [HideInInspector ]public float distanceToPlayer;//

    [Header("Game Objects")]
    public ParticleSystem engineParticle;//
    public ParticleSystem burnParticleSystem;//
    public GameObject _AOEreplacement;//
    public GameObject _burnReplacement;//
    public GameObject miniMapIcon;//
    public GameObject enemyMesh;//
    public GameObject dropItem;//
    public GameObject explosionObject; //TODO - Maybe i need only one of them
    public GameObject novaOnDie;//
  

    // gameObjects to find
    [HideInInspector] public Collider enemyCollider;//
    [HideInInspector] public GameManager gameManager;///
    [HideInInspector] public PlayerWeaponController playerWeaponController;//
    [HideInInspector] public NewEnemyBullet enemyBullet;//
    [HideInInspector] public Rigidbody playerRb;//
    [HideInInspector] public UpgradeChooseList upgradeChooseList;//

    [Header("Color Control")]
    public Color hitColor = new Color(1f, 0.6f, 0.0f, 1f);
    public Color critColor = new Color(1f, 0.0f, 1f, 1f);
    public Color burningColor = new Color(1f, 0.0f, 0.01f, 1f);
    [HideInInspector] public Color resultColor;



    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle methoden
    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameManager.OnDimensionSwap += HandleDimensionSwap;

        enemyBullet = gameObject.GetComponent<NewEnemyBullet>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        enemyCollider = GetComponent<Collider>();
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
        playerRb = playerWeaponController.gameObject.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (engineParticle != null) engineParticle.Play();
        InvokeRepeating("GetPlayerDistance", 0f, 0.25f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void GetPlayerDistance()
    {
        distanceToPlayer = DistanceToPlayer();
        Debug.Log(distanceToPlayer);
    }

    // Events
    public void HandleDimensionSwap(bool isSecondDimension = false)
    {
        Debug.Log("Handle Dimension Control");
        if (gameObject.activeSelf == true)
        {
            if (isSecondDimension == true)
            {
                isOutOfDimension = true;
                enemyCollider.enabled = false;
                if (miniMapIcon != null) miniMapIcon.SetActive(false);
                if (engineParticle != null) engineParticle.Stop();
                if (enemyBullet != null) enemyBullet.StopShooting();
                
            }
            else
            {
                isOutOfDimension = false;
                enemyCollider.enabled = true;
                if (miniMapIcon != null) miniMapIcon.SetActive(true);
                if (engineParticle != null && engineParticle.isPlaying == false) engineParticle.Play();
                if (enemyBullet != null) enemyBullet.StartShooting();
            }
        }
    }

    #endregion


    /* **************************************************************************** */
    /* MISC ----------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region Damage Control
    public void ShowDamageFromObjects(int damage)
    {
        Vector3 pos = transform.position;
        gameManager.DoFloatingText(pos, damage.ToString(), hitColor);
    }

    public void ShowDamageFromObjectsColor(int damage, Color hitColor_)
    {
        Vector3 pos = transform.position;
        gameManager.DoFloatingText(pos, damage.ToString(), hitColor_);
    }

    public void ShowDamageFromPosition(Vector3 pos, int damage)
    {
        gameManager.DoFloatingText(pos, damage.ToString(), new Color(1f, 0.6f, 0.0f, 1f));
    }


    // check player distance
    protected float DistanceToPlayer()
    {
        Vector2 playerPosition = new Vector2(playerRb.transform.position.x, playerRb.transform.position.z);
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);

        float distance = Vector2.Distance(playerPosition, currentPosition);

        return distance;
    }
    #endregion




}
