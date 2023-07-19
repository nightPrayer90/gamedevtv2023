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

[Header("GameObjects")]
    public Slider bossHealthSlider;
    public GameObject bossHud;
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
    private SphereCollider baseCollider;
    private Material[] materialList;
    private EnemyHealth enemyHealthScr;
    private Transform playerTr;
    private Rigidbody playerRb;
    private GameManager gameManager;
    private AudioSource bossChanceState;
   


    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        // hash components
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerTr = GameObject.FindWithTag("Player").transform;
        playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        baseCollider = gameObject.GetComponent<SphereCollider>();
        enemyHealthScr = gameObject.GetComponent<EnemyHealth>();
        bossChanceState = gameObject.GetComponent<AudioSource>();

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

        enemyHealthScr.DieEvent += EnemyHealthScr_DieEvent;
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
                    StateSleep();
                    break;

                case 1: // activate - fly to the fight start position

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
    private void StateSleep()
    {
        float distanceToPlayer = DistanceToPlayer();
        if (distanceToPlayer <= 3.5f)
        {
            bossState = 1;
            damageArea.SetActive(true);
            transform.DOMoveY(6, 5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                minimapIcon.transform.DOComplete();
                minimapSpR.DOColor(Color.red, 1f);

                // open bosshud
                bossHud.SetActive(true);
                bossHealthSlider.maxValue = enemyHealthScr.enemyHealth;
                bossHealthSlider.value = 0;

                AudioManager.Instance.PlaySFX("WarningBoss");
                transform.DOShakePosition(0.2f, 0.1f, 10, 90, false, true).OnComplete(() => 
                { 
                    ActivateState();
                    bossHealthSlider.DOValue(enemyHealthScr.enemyHealth, 1f);
                    damageArea.GetComponent<DamageArea>().FadeOut();
                });

            });
            gameManager.ScreenShake(3);
            AudioManager.Instance.PlaySFX("LiftUPBoss");
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
        // set activate material
        materialList[1] = emissivMaterial;
        bossMeshRenderer.materials = materialList;

        // go into fighting phase
        enemyHealthScr.canTakeDamage = true;
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
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);
                    //Debug.Log("state1 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() => 
                    { 
                        InvokeRepeating("Shooting2", 3f, 0.5f);
                        transform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
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
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);

                    //Debug.Log("state2 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() => 
                    { 
                        InvokeRepeating("Shooting1", 4f, 0.5f);
                        InvokeRepeating("Shooting2", 4f, 0.5f);
                        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
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

            transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 2f).SetDelay(2f);
            transform.DOShakePosition(4f, 0.3f, 20, 90, false, true).OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("BossExplode");
                rippleParticleDie.Play();
                PushThePlayer(6f, 6f);

                // replace
                Instantiate(replacement, transform.position, transform.rotation);
                bossMeshRenderer.enabled = false;
                baseCollider.enabled = false;

                // destroy the object
                Invoke("BossDelete",11f);
            });
        });

        bossState = 4;
    }

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
        AudioManager.Instance.PlaySFX("EnemyShootSound");
    }

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
}