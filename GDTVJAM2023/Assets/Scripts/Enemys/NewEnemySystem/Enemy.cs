using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject miniMapIcon;
    public GameObject enemyMesh;

    [Header("DieControll")]
    public GameObject _AOEreplacement;
    public GameObject _burnReplacement;

    // gameObjects to find
    protected Collider enemyCollider;
    protected GameManager gameManager;
    protected UpgradeChooseList upgradeChooseList;
    protected PlayerWeaponController playerWeaponController;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    #region lifecycle methoden
    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        upgradeChooseList = gameManager.GetComponent<UpgradeChooseList>();
        enemyCollider = GetComponent<Collider>();
        playerWeaponController = GameObject.FindWithTag("Player").GetComponent<PlayerWeaponController>();
 
        //collisionMultiplier += startCollisionMultiplier + UnityEngine.Random.Range(-16, 128);

    }

    private void OnEnable()
    {

    }
   


    #endregion




}
