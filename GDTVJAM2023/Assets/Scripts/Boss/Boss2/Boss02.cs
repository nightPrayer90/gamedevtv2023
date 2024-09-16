using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Boss02 : MonoBehaviour
{
    [Header("Boss Settings")]
    public int bossIndex = 0;
    public float followSpeed = 1f;
    private int bossState = 0;
    private int numberOfFightingStates = 3;
    private float fightingStatesStepSize = 0;
    private bool[] isState;
    private float dieRotation = 0;
    private bool isMinimap = false;
    [HideInInspector] public bool isDying = false;
    private int shieldObjects;
    private int maxShieldObjects = 5;


    [Header("GameObjects")]
    //public Material buildingMaterial;
    //public Material emissivMaterial;
    public MeshRenderer bossMeshRenderer;
    public ParticleSystem rippleParticle;
    public ParticleSystem rippleParticleDie;
    public List<Boss2SidePhase> laserWeapons = new List<Boss2SidePhase>();
    public List<Boss2SidePhase> laserWeapons2 = new List<Boss2SidePhase>();
    public ParticleSystem frontWeapon;
    public ParticleSystem frontWeapon2;
    public GameObject shootingWeapon;
    public GameObject shootingWeapon2;

    private int frontWeaponCount = 0;
    private int frontWeaponMaxCount = 5;
    private int frontWeaponCount2 = 0;
    private int frontWeaponMaxCount2 = 3;

    public GameObject explosionObject;
    public GameObject damageArea;
    public GameObject replacement;
    public Collider baseCollider;
    public AudioSource bossChanceState;
    public AudioSource shootSound;
    public GameObject itemDrop;
    private Material[] materialList;
    private EnemyHealth enemyHealthScr;
    private Transform playerTr;
    private Rigidbody playerRb;
    public Rigidbody rb;
    private GameManager gameManager;

    public Boss2upPhase upPhase;
    public Boss2upPhase2 upPhase2;
    public Boss2DownPhase downPhase;

    private Quaternion targetRotation;
    private BossUI bossUI;
    public BossMinimapIcon bossMinimapIcon;
    public BossParticle bossParticle;
    public Sprite bossUIForgroundSprite;


    /* **************************************************************************** */
    /* LIFECYCLE ------------------------------------------------------------------ */
    /* **************************************************************************** */
    #region LIFECYCLE
    private void Start()
    {
        // hash components
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerTr = GameObject.FindWithTag("Player").transform;
        playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        enemyHealthScr = gameObject.GetComponent<EnemyHealth>();
        enemyHealthScr.DieEvent += EnemyHealthScr_DieEvent;

        // set values
        enemyHealthScr.canTakeDamage = false;

        // update materials
        /*materialList = bossMeshRenderer.materials;
        materialList[0] = buildingMaterial;
        materialList[1] = buildingMaterial;
        bossMeshRenderer.materials = materialList;*/

        // fighting steps
        fightingStatesStepSize = enemyHealthScr.enemyHealth / numberOfFightingStates;
        isState = new bool[numberOfFightingStates];
        for (int i = 0; i < numberOfFightingStates; i++)
        {
            isState[i] = false;
        }

        // healthbar controll
        bossUI = gameManager.bossUI.GetComponent<BossUI>();

        // set tag for targeting weapons
        gameObject.tag = "Untagged";
    }

    private void EnemyHealthScr_DieEvent(object sender, System.EventArgs e)
    {
        CancelInvoke();
        bossState = 3;
    }

    private void Update()
    {
        if (gameManager.dimensionShift == true)
        {
            // Statemachine
            switch (bossState)
            {
                case 0: // sleeping - wait for player comes in range
                    SleepState();
                    break;

                case 1: // activate - fly to the fight start position
                    // triggered by dotween
                    break;

                case 2: // fight!
                    FightingState();
                    break;

                case 3: // die
                    DieState();
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        enemyHealthScr.DieEvent -= EnemyHealthScr_DieEvent;
    }
    #endregion


    /* **************************************************************************** */
    /* STATES ----------------------------------------------------------------- */
    /* **************************************************************************** */
    #region STATES
    private void SleepState()
    {
        float distanceToPlayer = DistanceToPlayer();
        if (distanceToPlayer <= 4.1f)
        {
            bossState = 1;
            bossParticle.ParticleStop();
            damageArea.SetActive(true);
            gameManager.ScreenShake(3);
            AudioManager.Instance.PlaySFX("LiftUPBoss");

            // open bosshud
            bossUI.InitHealthBar(enemyHealthScr.enemyHealth, bossUIForgroundSprite);
            bossUI.OpenBossUI();

            // fly to y = 6
            transform.DOMoveY(6, 5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                // set minimap to a red color
                bossMinimapIcon.SetIconToRed();

                rb.constraints = RigidbodyConstraints.FreezePositionX |
                             RigidbodyConstraints.FreezePositionY |
                             RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationZ;

                // go to the activate State
                AudioManager.Instance.PlaySFX("WarningBoss");
                transform.DOShakePosition(0.2f, 0.1f, 10, 90, false, true).OnComplete(() =>
                {
                    ActivateState();
                    damageArea.GetComponent<DamageArea>().FadeOut();
                });
            });
        }
        else
        {
            if (isMinimap == false && gameManager.districtNumber == bossIndex)
            {
                bossParticle.ParticleStart();
                bossMinimapIcon.InitMinimapIcon();
                isMinimap = true;
            }
        }
    }

    private void ActivateState()
    {
        // set tag for targeting weapons
        gameObject.tag = "secondDimensionEnemy";

        // set activate material
        //materialList[1] = emissivMaterial;
        //bossMeshRenderer.materials = materialList;
        downPhase.ActivateMesh();

        // go into fighting phase
        enemyHealthScr.canTakeDamage = true;
        bossUI.SetForgroundColor(Color.white);
        bossState = 2;
    }

    private void FightingState()
    {
        int currentState = Mathf.CeilToInt(enemyHealthScr.enemyHealth / fightingStatesStepSize);
        currentState = numberOfFightingStates - currentState;

        bossUI.UpdateSliderValue(enemyHealthScr.enemyHealth);

        switch (currentState)
        {
            case 0:
                // turns only one time per state
                if (isState[0] == false)
                {
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);
                    Shooting1();

                    downPhase.GoOnPosition();

                    InvokeRepeating(nameof(SpawnShield), 10f, 7.5f);

                    isState[0] = true;
                }
                RotateBoss(9f);
                MoveToPlayer(6f);
                RotateWeaponToPlayer(12);
                break;

            case 1:
                // turns only one time per state
                if (isState[1] == false)
                {
                    CancelInvoke(nameof(SpawnShield));
                    bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;
                    bossUI.SetForgroundColor(Color.red);
                    rippleParticle.Play();
                    upPhase.ActivateMesh();
                    upPhase.PhaseUP();

                    PushThePlayer(2.5f, 5f);
                    CancelInvoke();
                    Invoke(nameof(FrontWeaponReset), 1f);

                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");

                        Shooting2();
                        transform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossUI.SetForgroundColor(Color.white);

                        });
                    });
                    isState[1] = true;
                }
                RotateBoss(9f);
                RotateWeaponToPlayer(12);
                RotateWeaponToPlayer2(2);
                MoveToPlayer(6f);
                break;

            case 2:
                // turns only one time per state
                if (isState[2] == false)
                {
                    bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;
                    bossUI.SetForgroundColor(Color.red);
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);

                    downPhase.ActivateShield();

                    //Debug.Log("state2 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    Invoke(nameof(FrontWeaponReset), 1f);

                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");

                        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossUI.SetForgroundColor(Color.white);

                            upPhase2.ActivateMesh();


                        });
                    });
                    isState[2] = true;
                }
                RotateWeaponToPlayer(6);
                RotateWeaponToPlayer2(3);
                RotateBoss(12f);
                MoveToPlayer(6f);
                break;
        }
    }

    private void DieState()
    {
        bossUI.FadeOut();
        DestoryLaserWeapon();
        upPhase2.LaserStop();

        InvokeRepeating(nameof(InvokeSpawnExplosion), 0.5f, 1f);

        isDying = true;

        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
        {
            CancelInvoke();
            rippleParticle.Play();
            PushThePlayer(2.5f, 5f);
            bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
            bossChanceState.Play();

            downPhase.ShieldDie();

            // set activate material
            //materialList[1] = buildingMaterial;
            //bossMeshRenderer.materials = materialList;
            baseCollider.enabled = false;

            transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2f).SetDelay(2f);
            transform.DOShakePosition(4f, 0.3f, 20, 90, false, true).OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("BossExplode");
                rippleParticleDie.Play();
                PushThePlayer(6f, 6f);

                //drop
                ObjectPoolManager.SpawnObject(itemDrop, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

                // replace
                bossMinimapIcon.HideMinimapIcon();
                gameObject.tag = "Untagged";
                bossMeshRenderer.enabled = false;

                Instantiate(replacement, transform.position, replacement.transform.rotation);

                // destroy the object
                Invoke(nameof(BossDelete), 11f);

                // set tag for targeting weapons
                gameObject.tag = "Untagged";
            });
        });
        bossState = 4;
    }

    // invoke
    private void BossDelete()
    {
        Destroy(gameObject);
    }
    #endregion


    /* **************************************************************************** */
    /* HELP FUNCTIONS ------------------------------------------------------------- */
    /* **************************************************************************** */
    #region HELP FUNCTIONS
    // calculate the distance to the Player
    float DistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTr.transform.position);
        return distanceToPlayer;
    }

    // move to player is the player to far away
    private void MoveToPlayer(float distance)
    {
        float distanceToPlayer = DistanceToPlayer();

        if (distanceToPlayer >= distance)
        {
            Vector3 directionToPlayer = playerTr.position - transform.position;
            Vector3 targetPosition = playerTr.position - directionToPlayer.normalized * distance;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    // rotate the boss
    private void RotateBoss(float rotationSpeed)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    // push the player away from the boss
    private void PushThePlayer(float distence, float forcepower)
    {
        if (DistanceToPlayer() <= distence)
        {
            // push the player
            Vector3 pushDirection = playerTr.position - transform.position;
            Vector3 pushForceVector = pushDirection.normalized * forcepower;
            playerRb.AddForce(pushForceVector, ForceMode.Impulse);
        }
    }

    // invoke - shoot attack 1
    private void Shooting1()
    {
        int delay = 2;
        foreach (Boss2SidePhase weapon in laserWeapons)
        {
            weapon.ActivateWeapon(0, delay);
            delay += 6;
        }
    }

    // invoke - shoot attack 2
    private void Shooting2()
    {
        int delay = 6;
        foreach (Boss2SidePhase weapon in laserWeapons2)
        {
            weapon.ActivateWeapon(0, delay);
            delay += 6;
        }
    }

    // spawn Shields
    private void SpawnShield()
    {
        int delay = 0;
        shieldObjects += 2;
        foreach (Boss2SidePhase weapon in laserWeapons2)
        {
            weapon.ActivateWeapon(1, delay);
            delay += 4;
        }

        if (shieldObjects == maxShieldObjects) { CancelInvoke("SpawnShield"); }
    }

    // stops and destroy all laser
    private void DestoryLaserWeapon()
    {
        foreach (Boss2SidePhase weapon in laserWeapons)
        {
            weapon.LaserDie();
            weapon.ObjectDie();
        }
        foreach (Boss2SidePhase weapon in laserWeapons2)
        {
            weapon.LaserDie();
            weapon.ObjectDie();
        }
    }

    // invoke - last attack after die
    private void InvokeSpawnExplosion()
    {
        AudioManager.Instance.PlaySFX("ShieldGetHit");
        rippleParticle.Play();
        PushThePlayer(2.5f, 5f);
        transform.DOShakeScale(0.2f, 0.2f, 10, 90, true);
        dieRotation = dieRotation + 15;
        // instanstiate explosion
        ObjectPoolManager.SpawnObject(explosionObject, transform.position, Quaternion.Euler(0f, dieRotation, 0f), ObjectPoolManager.PoolType.ParticleSystem);
    }

    // control frontweapon
    private void RotateWeaponToPlayer(int frontWeaponMaxCount_)
    {
        Vector3 directionToPlayer = playerTr.position - shootingWeapon.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        targetRotation = Quaternion.Slerp(targetRotation, lookRotation, 2 * Time.deltaTime);

        if (Quaternion.Angle(shootingWeapon.transform.rotation, targetRotation) < 10)
        {
            if (frontWeaponCount == 0)
            {
                Invoke(nameof(InvokeShoot), 1f);
                frontWeaponCount = 1;
                frontWeaponMaxCount = frontWeaponMaxCount_;
            }
            return;
        }
        shootingWeapon.transform.rotation = targetRotation;
    }

    // invoke - shoot
    private void InvokeShoot()
    {
        shootSound.volume = AudioManager.Instance.sfxVolume;
        shootSound.Play();
        frontWeapon.Emit(1);
        frontWeaponCount += 1;

        if (frontWeaponCount <= frontWeaponMaxCount + 1)
        {
            Invoke(nameof(InvokeShoot), 0.6f);
        }
        else
        {
            Invoke(nameof(FrontWeaponReset), 3f);
        }
    }

    // invoke - reset the frontweapon 
    private void FrontWeaponReset()
    {
        frontWeaponCount = 0;
    }

    // control frontweapon
    private void RotateWeaponToPlayer2(int frontWeaponMaxCount_)
    {
        Vector3 directionToPlayer = playerTr.position - shootingWeapon2.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        targetRotation = Quaternion.Slerp(targetRotation, lookRotation, 3 * Time.deltaTime);

        if (Quaternion.Angle(shootingWeapon2.transform.rotation, targetRotation) < 2)
        {
            if (frontWeaponCount2 == 0)
            {
                Invoke(nameof(InvokeShoot2), 3f);
                frontWeaponCount2 = 1;
                frontWeaponMaxCount2 = frontWeaponMaxCount_;
            }
            return;
        }
        shootingWeapon2.transform.rotation = targetRotation;
    }

    // invoke - shoot
    private void InvokeShoot2()
    {
        shootSound.volume = AudioManager.Instance.sfxVolume;
        shootSound.Play();
        frontWeapon2.Emit(1);
        frontWeaponCount2 += 1;

        if (frontWeaponCount2 <= frontWeaponMaxCount2 + 1)
        {
            Invoke(nameof(InvokeShoot2), 2f);
        }
        else
        {
            Invoke(nameof(FrontWeaponReset2), 5f);
        }

    }

    // invoke - reset the frontweapon 
    private void FrontWeaponReset2()
    {
        frontWeaponCount2 = 0;
    }
    #endregion
}