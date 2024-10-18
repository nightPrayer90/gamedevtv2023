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
    //public Material buildingMaterial;
    //public Material emissivMaterial;
    public GameObject bossMesh01;
    public GameObject bossMesh02;
    public Transform bossMeshTransform;
    public MeshRenderer[] bossMeshRenderers;
    public ParticleSystem rippleParticle;
    public ParticleSystem rippleParticleDie;
    public List<ParticleSystem> particleWeapons = new List<ParticleSystem>();
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
    private GameManager gameManager;
    public PlayerData playerData;
    public BossParticle bossParticle;
    public BossMinimapIcon bossMinimapIcon;
    private BossUI bossUI;
    public Sprite bossUIForgroundSprite;
    public Boss01Ring bossRingController;
    public ParticleSystem windParticle;

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
                    RotateBossMesh(20f);
                    RotateBoss(10f);
                    bossRingController.RotateRing(50f);
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
    #endregion;


    /* **************************************************************************** */
    /* STATES --------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region STATES
    private void SleepState()
    {
        float distanceToPlayer = DistanceToPlayer();
        if (distanceToPlayer <= 5.8f)
        {
            bossState = 1;
            bossParticle.ParticleStop();
            damageArea.SetActive(true);
            gameManager.ScreenShake(3);
            AudioManager.Instance.PlaySFX("LiftUPBoss");

            // open bosshud
            bossUI.InitHealthBar(enemyHealthScr.enemyHealth, bossUIForgroundSprite);
            bossUI.OpenBossUI();

            // fly to y = 7
            transform.DOMoveY(6, 5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                // set minimap to a red color
                bossMinimapIcon.SetIconToRed();

                // go to the activate State
                AudioManager.Instance.PlaySFX("WarningBoss");
                bossMeshTransform.DOShakePosition(0.4f, 0.1f, 25, 90, false, true).SetDelay(0.2f).OnComplete(() =>
                {
                    damageArea.GetComponent<DamageArea>().FadeOut();
                    gameManager.ScreenShake(2);
                    ActivateState();

                    bossRingController.transform.DOShakePosition(1f, 0.08f, 17, 180, false, true);
                });
            });
        }
        else
        {
            if (isMinimap == false)
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
        /*materialList[1] = emissivMaterial;
        bossMeshRenderer.materials = materialList;*/

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
            case 0: // Attacke Phase 01
                // turns only one time per state
                if (isState[0] == false)
                {
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);
                    //Debug.Log("state0 @ " + enemyHealthScr.enemyHealth);

                    bossRingController.ShootingAttack01();
                    //InvokeRepeating(nameof(InvokeShootSound), 0.5f, 0.5f);
                    isState[0] = true;
                }
                RotateBoss(10f);
                bossRingController.RotateRing(20f);
                MoveToPlayer(6f);
                break;

            case 1: // Attacke Phase 02
                // turns only one time per state
                if (isState[1] == false)
                {
                    bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume,0.7f);
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;
                    bossUI.SetForgroundColor(Color.red);
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);
                    gameManager.ScreenShake(1);

                    //Debug.Log("state1 @ " + enemyHealthScr.enemyHealth);
                    bossRingController.StopShooting01();

                    CancelInvoke();
                    bossRingController.transform.DOShakePosition(0.5f, 0.1f, 10, 180, false, true);
                    bossMeshTransform.DOShakePosition(1f, 0.1f, 25, 90, false, true).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");

                        bossMeshTransform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossUI.SetForgroundColor(Color.white);
                            bossRingController.ShootingAttack02();
                        });
                    });
                    isState[1] = true;
                }
                RotateBoss(-12f);
                bossRingController.RotateRing(-25f);
                MoveToPlayer(6f);
                break;

            case 2: // Attacke Phase 03
                // turns only one time per state
                if (isState[2] == false)
                {
                    rippleParticle.Play();
                    bossMeshTransform.DOShakePosition(1f, 0.1f, 25, 90, false, true);
                    bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
                    bossChanceState.Play();
                    windParticle.Stop();

                    PushThePlayer(2.5f, 5f);
                    gameManager.ScreenShake(1);
                    CancelInvoke();

                    bossRingController.transform.DOShakePosition(1f, 0.06f, 30, 90, false, false);
                    bossRingController.Explode(); //1sek = Explode

                    Invoke(nameof(BossOpenPhase3),1.5f); //Boss open after Explode

                    enemyHealthScr.canTakeDamage = false;
                    bossUI.SetForgroundColor(Color.red);

                    bossMeshTransform.DOShakePosition(1f, 0.1f, 25, 90, false, true).SetDelay(2f).OnComplete(() =>
                    {
                        

                        bossMeshTransform.DOShakePosition(0.5f, 0.1f, 25, 90, false, true).OnComplete(() =>
                        {
                            AudioManager.Instance.PlaySFX("Boss01Angry");
                            bossMeshTransform.DOShakePosition(0.4f, 0.08f, 20, 90, false, true).SetDelay(1.8f).OnComplete(()=>AudioManager.Instance.PlaySFX("ShortAlert"));
                            InvokeRepeating(nameof(Shooting), 2f, 0.5f);
                            InvokeRepeating(nameof(InvokeShootSound), 2f, 0.5f);
                            enemyHealthScr.canTakeDamage = true;
                            bossUI.SetForgroundColor(Color.white);
                        });
                    });
                    isState[2] = true;
                }
                RotateBossMesh(50f);
                RotateBoss(10f);
                MoveToPlayer(6f);
                break;
        }
    }

    private void DieState()
    {
        AudioManager.Instance.PlaySFX("ShortAlert");

        bossUI.FadeOut();

        InvokeRepeating(nameof(InvokeSpawnExplosion), 0.5f, 1f);

        Invoke(nameof(BossCloseBeforeExplode), 3);

        bossMeshTransform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
        {
            CancelInvoke();
            rippleParticle.Play();
            PushThePlayer(2.5f, 5f);
            bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
            bossChanceState.Play();

            AudioManager.Instance.PlaySFX("ShieldGetHit");

            // set activate material
            /*materialList[1] = buildingMaterial;
            bossMeshRenderer.materials = materialList;*/

            bossMeshTransform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2f).SetDelay(2f);
            bossMeshTransform.DOShakePosition(4f, 0.3f, 20, 90, false, true).OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("BossExplode");
                gameManager.ScreenShake(6);
                rippleParticleDie.Play();
                gameObject.tag = "Untagged";
                PushThePlayer(6f, 6f);


                // drop
                ObjectPoolManager.SpawnObject(itemDrop, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

                // replace
                bossMinimapIcon.HideMinimapIcon();
                baseCollider.enabled = false;
                Instantiate(replacement, transform.position, transform.rotation);
                bossMeshRenderers[0].enabled = false;
                bossMeshRenderers[1].enabled = false;
                bossMeshRenderers[2].enabled = false;

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

    // rotate 
    private void RotateBoss(float rotationSpeed)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void RotateBossMesh(float rotationSpeed)
    {
        bossMeshTransform.rotation = bossMeshTransform.rotation * Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
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
    private void Shooting()
    {
        foreach (ParticleSystem weapon in particleWeapons)
        {
            weapon.Emit(1);
        }
    }

    // invoke - shoot sound
    private void InvokeShootSound()
    {
        shootSound.volume = AudioManager.Instance.sfxVolume;
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

    public void BossOpenPhase3()
    {
        AudioManager.Instance.PlaySFX("Boss01open");
        
        bossMesh01.transform.DOLocalMoveY(0.06f, 2.2f).SetDelay(0.4f);
        bossMesh02.transform.DOLocalMoveY(-0.06f, 2.2f).SetDelay(0.2f).OnComplete(()=> windParticle.Play());
    }

    public void BossCloseBeforeExplode()
    {
        AudioManager.Instance.PlaySFX("Boss01close");
        windParticle.Stop();
        bossMesh01.transform.DOLocalMoveY(0f, 3.2f).SetDelay(0.2f);
        bossMesh02.transform.DOLocalMoveY(0f, 3.2f).SetDelay(0.3f);
    }
    #endregion
}