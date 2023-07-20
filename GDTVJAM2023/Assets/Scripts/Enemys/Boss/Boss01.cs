using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Boss01 : MonoBehaviour
{
    [Header("Boss Settings")]
    public float followSpeed = 1f;
    private int bossState = 0;
    private int numberOfFightingStates = 3;
    private float fightingStatesStepSize = 0;
    private bool[] isState;
    private float dieRotation = 0;
    private bool isMinimap = false;


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
    public List<ParticleSystem> particleWeapons = new List<ParticleSystem>();
    public List<ParticleSystem> particleWeapons2 = new List<ParticleSystem>();
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
    private GameManager gameManager;
    



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
        gameObject.tag = "Default";

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
        if (distanceToPlayer <= 3.5f)
        {
            bossState = 1;
            damageArea.SetActive(true);
            gameManager.ScreenShake(3);
            AudioManager.Instance.PlaySFX("LiftUPBoss");

            // open bosshud
            bossHud.SetActive(true);
            bossHudCg.DOFade(1f, 0.2f);
            bossHealthSlider.maxValue = enemyHealthScr.enemyHealth;
            bossHealthSlider.value = 0;
            bossHealthSlider.DOValue(enemyHealthScr.enemyHealth, 5.2f).OnComplete(() =>
            {
                bossHealthSlider.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 1.5f, 10, 1f);
            });

            // fly to y = 7
            transform.DOMoveY(6, 5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                // set minimap to a red color
                minimapIcon.transform.DOComplete();
                minimapIcon.transform.DOKill();
                minimapSpR.DOColor(Color.red, 1f);


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
            if (isMinimap == false)
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
                    //Debug.Log("state0 @ " + enemyHealthScr.enemyHealth);
                    InvokeRepeating("Shooting1",0.5f,0.5f);
                    InvokeRepeating("InvokeShootSound", 0.5f, 0.5f);
                    isState[0] = true;
                }

                RotateBoss(10f);
                MoveToPlayer(6f);
                break;

            case 1:
                // turns only one time per state
                if (isState[1] == false)
                {
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;
                    bossHealthForeground.color = Color.red;
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);
                    //Debug.Log("state1 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() => 
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");
                        InvokeRepeating("Shooting2", 3f, 0.5f);
                        InvokeRepeating("InvokeShootSound", 3f, 0.5f);
                        transform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossHealthForeground.color = Color.white;
                        });
                    });
                    isState[1] = true;
                }

                RotateBoss(-13f);
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

                    //Debug.Log("state2 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() => 
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");
                        InvokeRepeating("Shooting1", 4f, 0.5f);
                        InvokeRepeating("Shooting2", 4f, 0.5f);
                        InvokeRepeating("InvokeShootSound", 4f, 0.5f);
                        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossHealthForeground.color = Color.white;
                        });
                    });
                    isState[2] = true;
                }

                RotateBoss(10f);
                MoveToPlayer(6f);
                break;
        }
    }

    private void DieState()
    {
        bossHudCg.DOFade(0f, 0.5f).OnComplete(()=> { bossHud.SetActive(false); });

        InvokeRepeating("InvokeSpawnExplosion", 0.5f, 1f);
        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
        {
            CancelInvoke();
            rippleParticle.Play();
            PushThePlayer(2.5f, 5f);
            bossChanceState.Play();

            // set activate material
            materialList[1] = buildingMaterial;
            bossMeshRenderer.materials = materialList;

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
                baseCollider.enabled = false;
                Instantiate(replacement, transform.position, transform.rotation);
                bossMeshRenderer.enabled = false;
                
                // destroy the object
                Invoke("BossDelete",11f);

                // set tag for targeting weapons
                gameObject.tag = "Default";
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

    // rotate 
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
        foreach (ParticleSystem weapon in particleWeapons)
        {
            weapon.Emit(1);

        }
    }

    // invoke - shoot attack 2
    private void Shooting2()
    {
        foreach (ParticleSystem weapon in particleWeapons2)
        {
            weapon.Emit(1);
        }
    }

    // invoke - shoot sound
    private void InvokeShootSound()
    {
        shootSound.Play();
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
}