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
    private int maxShieldObjects = 8;


    [Header("Boss UI")]
    public GameObject bossHud;
    public CanvasGroup bossHudCg;
    public Slider bossHealthSlider;
    public Image bossHealthForeground;
    public Image bossHealthFillarea;
    public Sprite bossHealthForgroundSp;
    public Sprite bossHealthFillareaSp;


    [Header("GameObjects")]
    public Material buildingMaterial;
    public Material emissivMaterial;
    public MeshRenderer bossMeshRenderer;
    public ParticleSystem rippleParticle;
    public ParticleSystem rippleParticleDie;
    public List<Boss2SidePhase> laserWeapons = new List<Boss2SidePhase>();
    public List<Boss2SidePhase> laserWeapons2 = new List<Boss2SidePhase>();
    public ParticleSystem frontWeapon;
    public ParticleSystem frontWeapon2;
    public GameObject shootingWeapon;
    public GameObject shootingWeapon2;

    private int frontWeaponCount= 0;
    private int frontWeaponMaxCount = 5;
    private int frontWeaponCount2 = 0;
    private int frontWeaponMaxCount2 = 3;

    public GameObject explosionObject;
    public GameObject minimapIcon;
    public SpriteRenderer minimapSpR;
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


    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
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
        materialList = bossMeshRenderer.materials;
        materialList[0] = buildingMaterial;
        materialList[1] = buildingMaterial;
        bossMeshRenderer.materials = materialList;

        // fighting steps
        fightingStatesStepSize = enemyHealthScr.enemyHealth / numberOfFightingStates;
        isState = new bool[numberOfFightingStates];
        for (int i = 0; i < numberOfFightingStates; i++)
        {
            isState[i] = false;
        }

        // healthbar controll
        bossHudCg.alpha = 0;
        bossHealthForeground.sprite = bossHealthForgroundSp;
        bossHealthFillarea.sprite = bossHealthFillareaSp;
        bossHealthForeground.color = Color.red;

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



    /* **************************************************************************** */
    /* BOSS STATE ----------------------------------------------------------------- */
    /* **************************************************************************** */
    private void SleepState()
    {
        float distanceToPlayer = DistanceToPlayer();
        if (distanceToPlayer <= 4.1f)
        {
            bossState = 1;
            damageArea.SetActive(true);
            gameManager.ScreenShake(3);
            AudioManager.Instance.PlaySFX("LiftUPBoss");

            // open bosshud
            bossHud.SetActive(true);
            bossHudCg.DOFade(1f, 0.2f);
            bossHealthForeground.sprite = bossHealthForgroundSp;
            bossHealthSlider.maxValue = enemyHealthScr.enemyHealth;
            bossHealthSlider.value = 0;
            bossHealthSlider.DOValue(enemyHealthScr.enemyHealth, 5.2f).OnComplete(() =>
            {
                bossHealthSlider.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1.5f, 10, 1f);
            });

            // fly to y = 6
            transform.DOMoveY(6, 5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                // set minimap to a red color
                minimapIcon.transform.DOComplete();
                minimapIcon.transform.DOKill();
                minimapSpR.DOColor(Color.red, 1f);

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
                minimapIcon.SetActive(true);

                minimapSpR.DOFade(1f, 2f).SetDelay(1f);
                minimapIcon.transform.DOScale(new Vector3(15f, 15f, 15f), 2f).SetDelay(1f).OnComplete(() =>
                {
                    minimapIcon.transform.DOPunchScale(new Vector3(7f, 7f, 7f), 2f, 1, 0.4f).SetDelay(2f).SetLoops(-1, LoopType.Restart);
                });


                isMinimap = true;
            }
        }

    }

    private void ActivateState()
    {
        // set tag for targeting weapons
        gameObject.tag = "secondDimensionEnemy";

        // set activate material
        materialList[1] = emissivMaterial;
        bossMeshRenderer.materials = materialList;
        downPhase.ActivateMesh();

        // go into fighting phase
        enemyHealthScr.canTakeDamage = true;
        bossHealthForeground.color = Color.white;
        bossState = 2;
    }

    private void FightingState()
    {
        int currentState = Mathf.CeilToInt(enemyHealthScr.enemyHealth / fightingStatesStepSize);
        currentState = numberOfFightingStates - currentState;

        bossHealthSlider.value = enemyHealthScr.enemyHealth;

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

                    InvokeRepeating("SpawnShield", 10f, 7.5f);
                  
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
                    CancelInvoke("SpawnShield");
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;
                    bossHealthForeground.color = Color.red;
                    rippleParticle.Play();
                    upPhase.ActivateMesh();
                    upPhase.PhaseUP();

                    

                    PushThePlayer(2.5f, 5f);
                    //Debug.Log("state1 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    Invoke("FrontWeaponReset", 1f);

                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() => 
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");
                        //InvokeRepeating("Shooting2", 3f, 0.5f);
                        //InvokeRepeating("InvokeShootSound", 3f, 0.5f);

                       
                        Shooting2();
                        transform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossHealthForeground.color = Color.white;
                            
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
                    bossChanceState.Play( );
                    enemyHealthScr.canTakeDamage = false;
                    bossHealthForeground.color = Color.red;
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);

                    downPhase.ActivateShield();

                    //Debug.Log("state2 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    Invoke("FrontWeaponReset", 1f);

                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() => 
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");
                        
                        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossHealthForeground.color = Color.white;

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
        bossHudCg.DOFade(0f, 0.5f).OnComplete(()=> { bossHud.SetActive(false); });
        DestoryLaserWeapon();
        upPhase2.LaserStop();

        InvokeRepeating("InvokeSpawnExplosion", 0.5f, 1f);
        
        isDying = true;

        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
        {
            CancelInvoke();
            rippleParticle.Play();
            PushThePlayer(2.5f, 5f);
            bossChanceState.Play();

            downPhase.ShieldDie();

            // set activate material
            materialList[1] = buildingMaterial;
            bossMeshRenderer.materials = materialList;
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
                minimapIcon.SetActive(false);            
                gameObject.tag = "Untagged";
                bossMeshRenderer.enabled = false;

                Instantiate(replacement, transform.position, replacement.transform.rotation);
                
                // destroy the object
                Invoke("BossDelete",11f);

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




    /* **************************************************************************** */
    /* HELP FUNCTIONS ------------------------------------------------------------- */
    /* **************************************************************************** */
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
        transform.rotation = transform.rotation * Quaternion.Euler( 0f , rotationSpeed * Time.deltaTime, 0f);
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

        if (shieldObjects == maxShieldObjects) { CancelInvoke("SpawnShield");  }
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

    // control the frontweapon
    private void RotateWeaponToPlayer(int frontWeaponMaxCount_)
    {
        Vector3 directionToPlayer = playerTr.position - shootingWeapon.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        // Verzögere die Ausrichtung des Objekts mit einer Rotationsgeschwindigkeit
        targetRotation = Quaternion.Slerp(targetRotation, lookRotation, 2 * Time.deltaTime);

        if (Quaternion.Angle(shootingWeapon.transform.rotation, targetRotation) < 10)
        {
            if (frontWeaponCount == 0)
            {
                Invoke("InvokeShoot", 1f);
                frontWeaponCount = 1;
                frontWeaponMaxCount = frontWeaponMaxCount_;
            }
            return; // Beende die Aktualisierung der Ausrichtung
        }

        // Wende die Zielrotation auf das Objekt an
        shootingWeapon.transform.rotation = targetRotation;
    }

    // invoke - shoot
    private void InvokeShoot()
    {
        shootSound.Play();
        frontWeapon.Emit(1);
        frontWeaponCount += 1;

        if (frontWeaponCount <= frontWeaponMaxCount+1)
        {
            Invoke("InvokeShoot", 0.6f);
        }
        else
        {
            Invoke("FrontWeaponReset",3f);
        }

    }
    
    // invoke - reset the frontweapon 
    private void FrontWeaponReset()
    {
        frontWeaponCount = 0;
    }

    // control the frontweapon
    private void RotateWeaponToPlayer2(int frontWeaponMaxCount_)
    {
        Vector3 directionToPlayer = playerTr.position - shootingWeapon2.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

        targetRotation = Quaternion.Slerp(targetRotation, lookRotation, 3 * Time.deltaTime);

        if (Quaternion.Angle(shootingWeapon2.transform.rotation, targetRotation) < 2)
        {
            if (frontWeaponCount2 == 0)
            {
                Invoke("InvokeShoot2", 3f);
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
        shootSound.Play();
        frontWeapon2.Emit(1);
        frontWeaponCount2 += 1;

        if (frontWeaponCount2 <= frontWeaponMaxCount2 + 1)
        {
            Invoke("InvokeShoot2", 2f);
        }
        else
        {
            Invoke("FrontWeaponReset2", 5f);
        }

    }

    // invoke - reset the frontweapon 
    private void FrontWeaponReset2()
    {
        frontWeaponCount2 = 0;
    }
}