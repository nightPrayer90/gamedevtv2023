using UnityEngine;
using DG.Tweening;
using System;

public class Boss04 : MonoBehaviour
{
    [Header("Boss Settings")]
    public int bossIndex = 4;
    private int bossState = 0;
    private int numberOfFightingStates = 2;
    private float fightingStatesStepSize = 0;
    private bool[] isState;
    private float dieRotation = 0;
    private bool isMinimap = false;

    [Header("GameObjects")]
    public MeshRenderer bossMeshRenderer;
    public ParticleSystem rippleParticle;
    public ParticleSystem rippleParticleDie;
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
    private Rigidbody bossRB;
    public GameObject damageZonePrefab;
    private Boss04DamageZone damageZone;
    public BossModulMainengine engineParticle;
    public BossModulMainengine attack2EngineParticle;
    public Boss04DropAttack dropAttack;
    public Sprite bossUIForgroundSprite;

    private bool targetSet = true;
    private bool resetSet = true;
    private bool attackFlag = false;
    private int attackType = 0;
    private Vector3 targetPosition;
    private int lastAttack = -1;

    [Header("Weapon")]
    public Laser2[] lasers;
    private int laserIndex = 0;
    public ParticleSystem[] bulletshooters;

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
        bossRB = gameObject.GetComponent<Rigidbody>();
        enemyHealthScr = gameObject.GetComponent<EnemyHealth>();
        enemyHealthScr.DieEvent += EnemyHealthScr_DieEvent;

        // set values
        enemyHealthScr.canTakeDamage = false;

        // update materials
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
    #endregion;


    /* **************************************************************************** */
    /* STATES --------------------------------------------------------------------- */
    /* **************************************************************************** */
    #region STATES
    private void SleepState()
    {
        float distanceToPlayer = DistanceToPlayer();
        if (distanceToPlayer <= 6f)
        {
            bossState = 1;
            bossParticle.ParticleStop();
            damageArea.SetActive(true);
            gameManager.ScreenShake(3);
            AudioManager.Instance.PlaySFX("LiftUPBoss");
            engineParticle.EngineActivate();

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

        // go into fighting phase
        enemyHealthScr.canTakeDamage = true;
        bossUI.SetForgroundColor(Color.white);
        bossState = 2;
        Invoke(nameof(ResetrargetSetFlag), 2);
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
                    PushThePlayer(2.5f, 5f, rippleParticle);
                    isState[0] = true;
                }

                ControllAttackMovment(3);
                break;

            case 1:
                // turns only one time per state
                if (isState[1] == false)
                {
                    if (damageZone != null) { damageZone.Destroy(); damageZone = null; }

                    bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;

                    bossUI.SetForgroundColor(Color.red);
                    PushThePlayer(2.5f, 5f, rippleParticle);
                    CancelInvoke();

                    InvokeRepeating(nameof(FireBullets), 4f, 0.1f);

                    transform.DORotate(new Vector3(0f, 1000f, 0f), 3f, RotateMode.FastBeyond360).SetDelay(4f).OnComplete(() =>
                    {
                        CancelInvoke();
                        AudioManager.Instance.PlaySFX("ShieldGetHit");
                        transform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            Invoke(nameof(ResetrargetSetFlag), 3f);
                            enemyHealthScr.canTakeDamage = true;
                            bossUI.SetForgroundColor(Color.white);
                        });
                    });
                    isState[1] = true;
                }

                ControllAttackMovment(6);
                break;
        }
    }

    private void DieState()
    {
        bossUI.FadeOut();
        engineParticle.EngineStop();
        attack2EngineParticle.EngineStop();
        if (damageZone != null) { damageZone.DestroyObject(); damageZone = null; }


        transform.DOShakePosition(1f, 0.2f, 10, 90, false, true);

        InvokeRepeating(nameof(FireBullets), 2.5f, 0.1f);
        dropAttack.SpawnFallingObjects(35);

        transform.DORotate(new Vector3(0f, 1000f, 0f), 3f, RotateMode.FastBeyond360).SetDelay(2.5f).OnComplete(() =>
        {
            CancelInvoke();
            PushThePlayer(2.5f, 5f, rippleParticle);
            bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
            bossChanceState.Play();

            transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2f).SetDelay(2f);
            transform.DOShakePosition(4f, 0.3f, 20, 90, false, true).OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("BossExplode");
                gameObject.tag = "Untagged";
                PushThePlayer(6f, 6f, rippleParticleDie);

                // drop
                ObjectPoolManager.SpawnObject(itemDrop, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

                // replace
                bossMinimapIcon.HideMinimapIcon();
                baseCollider.enabled = false;
                Instantiate(replacement, transform.position, transform.rotation);
                bossMeshRenderer.gameObject.SetActive(false);

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

    // push the player away from the boss
    private void PushThePlayer(float distence, float forcepower, ParticleSystem rippleParticle)
    {
        if (!rippleParticle.isPlaying) { rippleParticle.Play(); AudioManager.Instance.PlaySFX("ShieldGetHit"); }
        if (DistanceToPlayer() <= distence)
        {
            // push the player
            Vector3 pushDirection = playerTr.position - transform.position;
            Vector3 pushForceVector = pushDirection.normalized * forcepower;
            playerRb.AddForce(pushForceVector, ForceMode.Impulse);
        }
    }

    // invoke - last attack after die
    private void InvokeSpawnExplosion()
    {
        AudioManager.Instance.PlaySFX("ShieldGetHit");
        PushThePlayer(2.5f, 5f, rippleParticle);
        transform.DOShakeScale(0.2f, 0.2f, 10, 90, true);
        dieRotation = dieRotation + 15;
        // instanstiate explosion
        ObjectPoolManager.SpawnObject(explosionObject, transform.position, Quaternion.Euler(0f, dieRotation, 0f), ObjectPoolManager.PoolType.ParticleSystem);
    }

    // Attack State - MovmentStuff
    private void ControllAttackMovment(int attacktype)
    {
        if (attackFlag == false)
        {
            if (lastAttack != -1)
            {
                if (UnityEngine.Random.Range(0, 20) > 10) AudioManager.Instance.PlaySFX("Boss4Speak1");
                else AudioManager.Instance.PlaySFX("Boss4Speak2");
            }
            attackFlag = true;

            int starttype = 0;
            if (attacktype == 6) starttype = 2;

            while (attackType == lastAttack)
            {
                attackType = UnityEngine.Random.Range(starttype, attacktype);
            }

            float distance = DistanceToPlayer();

            if (distance > 10) attackType = 0;
            lastAttack = attackType;
        }

        switch (attackType)
        {
            // move to near random position
            case 0:
                MoveTowardsTarget(4.5f, 1);
                break;

            // rotate and fire bullets
            case 1:
                RotateBulletAttack(25f);
                break;

            // move to player position
            case 2:
                MoveTowardsTarget(5f, 0);
                break;


            // follow player and rotate
            case 3:
                FollowPlayer(2f);
                break;

            // rotate fast and fire bullets
            case 4:
                RotateBulletAttack(70f);
                break;

            // move to near random position and fire Drop Rockets
            case 5:
                MoveTowardsTarget(4f, 2);
                break;

        }

        // Faceing the boss in Player direction
        if (targetSet == true && resetSet == true)
        {
            RotateTowardsPlayer(120f);
        }
    }

    // Attack 1 - Move to Player Position
    // ---------
    private void SpawnDamageZone(int type)
    {
        if (type == 0)
        {
            Invoke(nameof(SetTargetPosition), 2f);

            GameObject go = Instantiate(damageZonePrefab, playerRb.position, transform.rotation);
            damageZone = go.GetComponent<Boss04DamageZone>();
            targetSet = true;
            resetSet = true;
        }
        else
        {
            targetPosition = transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
            targetSet = true;
            resetSet = false;
        }
    }

    private void SetTargetPosition()
    {
        resetSet = false;
        targetPosition = playerRb.position;
        if (damageZone != null) damageZone.trackToPlayer = false;
    }


    private void MoveTowardsTarget(float speed, int type)
    {
        if (targetSet == false)
        {
            SpawnDamageZone(type);
        }
        else if (targetSet == true && resetSet == false)
        {
            Vector3 direction = (targetPosition - transform.position);
            direction.Normalize();

            Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

            transform.position = newPosition;

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                if (type == 0 || type == 1)
                {
                    if (damageZone != null) { damageZone.DestroyObject(); damageZone = null; }
                    if (type == 1)
                    {
                        FireBullets();
                    }
                    dropAttack.SpawnFallingObjects(10);
                    transform.DOComplete();
                    transform.DOShakePosition(0.5f, 0.4f, 10, 90, false, true);
                    PushThePlayer(3f, 5f, rippleParticle);
                    resetSet = true;
                    Invoke(nameof(ResetrargetSetFlag), 3f);
                }
                else if (type == 2)
                {
                    if (damageZone != null) { damageZone.DestroyObject(); damageZone = null; }
                    transform.DOComplete();
                    dropAttack.SpawnFallingObjects(25);
                    transform.DORotate(new Vector3(01f, 1300f, 0f), 4f, RotateMode.FastBeyond360);
                    PushThePlayer(3f, 5f, rippleParticle);
                    resetSet = true;
                    Invoke(nameof(ResetrargetSetFlag), 6f);
                }
                bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
                bossChanceState.Play();
            }
        }
    }

    void RotateTowardsPlayer(float rotateSpeed)
    {
        Vector3 direction = playerRb.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    private void ResetrargetSetFlag()
    {
        attackFlag = false;
        targetSet = false;
        resetSet = false;
        transform.DOComplete();
        transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true);
    }
    // ---------
    // Attack 2 - Follow Player and Rotate
    private void FollowPlayer(float speed)
    {
        if (targetSet == false)
        {
            laserIndex = 0;
            Invoke(nameof(ResetSetFlagA2), 5.5f);
            Invoke(nameof(ActivateLaser), 0.3f);
            Invoke(nameof(DeactivateLaser), 4.5f);
            attack2EngineParticle.EngineActivate();
            targetSet = true;
            AudioManager.Instance.PlaySFX("Boss4LaserAttack");
        }

        if (resetSet == false)
        {
            Vector3 direction = (playerRb.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Rotate around Y-Axis
            transform.Rotate(Vector3.up, 210f * Time.deltaTime);

            if (DistanceToPlayer() < 1.7)
            {
                PushThePlayer(1.8f, 5f, rippleParticle);
            }
        }
    }

    private void ActivateLaser()
    {
        if (laserIndex < lasers.Length)
        {
            //AudioManager.Instance.PlaySFX("PlayerMineDetect");
            lasers[laserIndex].LaserActivate();
            laserIndex += 1;
            Invoke(nameof(ActivateLaser), 0.2f);
        }
        else
        {
            laserIndex = 0;
        }
    }

    private void DeactivateLaser()
    {
        if (laserIndex < lasers.Length)
        {
            AudioManager.Instance.PlaySFX("Playernova");
            lasers[laserIndex].LaserStop();
            laserIndex += 1;
            Invoke(nameof(DeactivateLaser), 0.2f);
        }
        attack2EngineParticle.EngineStop();
    }

    private void ResetSetFlagA2()
    {
        laserIndex = 0;
        resetSet = true;

        Invoke(nameof(ResetrargetSetFlag), 4f);
    }
    // ---------
    // Attack 3 - Rotate and FireBullets
    private void RotateBulletAttack(float rotateSpeed)
    {
        if (targetSet == false)
        {
            laserIndex = 0;
            InvokeRepeating(nameof(FireBullets), 0.3f, 0.3f);
            Invoke(nameof(ResetSetFlagA3), 8f);
            attack2EngineParticle.EngineActivate();

            targetSet = true;
        }

        if (resetSet == false)
        {
            // Rotate around Y-Axis
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }

    }

    private void FireBullets()
    {
        shootSound.volume = AudioManager.Instance.sfxVolume;
        shootSound.Play();
        foreach (ParticleSystem shooter in bulletshooters)
        {
            shooter.Emit(1);
        }
    }

    private void ResetSetFlagA3()
    {
        attack2EngineParticle.EngineStop();
        resetSet = true;
        CancelInvoke(nameof(FireBullets));
        Invoke(nameof(ResetrargetSetFlag), 4f);
    }
    #endregion
}