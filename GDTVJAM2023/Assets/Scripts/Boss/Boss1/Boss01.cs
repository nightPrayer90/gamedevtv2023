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
    public Material buildingMaterial;
    public Material emissivMaterial;
    public MeshRenderer bossMeshRenderer;
    public ParticleSystem rippleParticle;
    public ParticleSystem rippleParticleDie;
    public List<ParticleSystem> particleWeapons = new List<ParticleSystem>();
    public List<ParticleSystem> particleWeapons2 = new List<ParticleSystem>();
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
        bossUI = gameManager.bossUI.GetComponent<BossUI>();
        bossUI.InitHealthBar(enemyHealthScr.enemyHealth);

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

            // open bosshud
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
        materialList[1] = emissivMaterial;
        bossMeshRenderer.materials = materialList;

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
                    //Debug.Log("state0 @ " + enemyHealthScr.enemyHealth);
                    InvokeRepeating(nameof(Shooting1), 0.5f, 0.5f);
                    InvokeRepeating(nameof(InvokeShootSound), 0.5f, 0.5f);
                    isState[0] = true;
                }
                RotateBoss(10f);
                MoveToPlayer(6f);
                break;

            case 1:
                // turns only one time per state
                if (isState[1] == false)
                {
                    bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume,0.7f);
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;
                    bossUI.SetForgroundColor(Color.red);
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);
                    //Debug.Log("state1 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");
                        InvokeRepeating(nameof(Shooting2), 3f, 0.5f);
                        InvokeRepeating(nameof(InvokeShootSound), 3f, 0.5f);
                        transform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossUI.SetForgroundColor(Color.white);
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
                    bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
                    bossChanceState.Play();
                    enemyHealthScr.canTakeDamage = false;
                    bossUI.SetForgroundColor(Color.red);
                    rippleParticle.Play();
                    PushThePlayer(2.5f, 5f);

                    //Debug.Log("state2 @ " + enemyHealthScr.enemyHealth);
                    CancelInvoke();
                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySFX("ShieldGetHit");
                        InvokeRepeating(nameof(Shooting1), 4f, 0.5f);
                        InvokeRepeating(nameof(Shooting2), 4f, 0.5f);
                        InvokeRepeating(nameof(InvokeShootSound), 4f, 0.5f);
                        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                            bossUI.SetForgroundColor(Color.white);
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
        bossUI.FadeOut();

        InvokeRepeating(nameof(InvokeSpawnExplosion), 0.5f, 1f);
        transform.DOShakePosition(4f, 0.1f, 10, 90, false, true).OnComplete(() =>
        {
            CancelInvoke();
            rippleParticle.Play();
            PushThePlayer(2.5f, 5f);
            bossChanceState.volume = Mathf.Min(AudioManager.Instance.sfxVolume, 0.7f);
            bossChanceState.Play();

            // set activate material
            materialList[1] = buildingMaterial;
            bossMeshRenderer.materials = materialList;

            transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2f).SetDelay(2f);
            transform.DOShakePosition(4f, 0.3f, 20, 90, false, true).OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("BossExplode");
                rippleParticleDie.Play();
                gameObject.tag = "Untagged";
                PushThePlayer(6f, 6f);

                // drop
                ObjectPoolManager.SpawnObject(itemDrop, transform.position, transform.rotation, ObjectPoolManager.PoolType.PickUps);

                // replace
                bossMinimapIcon.HideMinimapIcon();
                baseCollider.enabled = false;
                Instantiate(replacement, transform.position, transform.rotation);
                bossMeshRenderer.enabled = false;

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
    #endregion
}