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


[Header("GameObjects")]
    public Slider bossHealthSlider;
    public GameObject bossHud;
    public Material buildingMaterial;
    public Material emissivMaterial;
    public MeshRenderer bossMeshRenderer;
    public ParticleSystem rippleParticle;
    public List<ParticleSystem> particleWeapons = new List<ParticleSystem>();
    public List<ParticleSystem> particleWeapons2 = new List<ParticleSystem>();
    private Material[] materialList;
    private EnemyHealth enemyHealthScr;
    private Transform playerTr;
    private GameManager gameManager;




    /* **************************************************************************** */
    /* Lifecycle-Methoden---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Start()
    {
        // hash components
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerTr = GameObject.FindWithTag("Player").transform;
        enemyHealthScr = gameObject.GetComponent<EnemyHealth>();

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
            transform.DOMoveY(6, 5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                // open bosshud
                bossHud.SetActive(true);
                bossHealthSlider.maxValue = enemyHealthScr.enemyHealth;
                bossHealthSlider.value = 0;

                AudioManager.Instance.PlaySFX("WarningBoss");
                transform.DOShakePosition(0.2f, 0.1f, 10, 90, false, true).OnComplete(() => 
                { 
                    ActivateState();
                    bossHealthSlider.DOValue(enemyHealthScr.enemyHealth, 1f);
                });

            });
            gameManager.ScreenShake(3);
            AudioManager.Instance.PlaySFX("LiftUPBoss");
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
                    Debug.Log("state0 @ " + enemyHealthScr.enemyHealth);
                    isState[0] = true;
                    InvokeRepeating("Shooting1",0.5f,0.5f);
                }

                RotateBoss(10f);
                MoveToPlayer(8f);
                break;

            case 1:
                // turns only one time per state
                if (isState[1] == false)
                {
                    AudioManager.Instance.PlaySFX("Explosion");
                    enemyHealthScr.canTakeDamage = false;
                    rippleParticle.Play();
                    Debug.Log("state1 @ " + enemyHealthScr.enemyHealth);
                    isState[1] = true;
                    CancelInvoke();
                    transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnComplete(() => 
                    { 
                        InvokeRepeating("Shooting2", 3f, 0.5f);
                        transform.DOShakePosition(3f, 0.1f, 10, 90, false, true).OnComplete(() =>
                        {
                            enemyHealthScr.canTakeDamage = true;
                        });
                    });
                    
                }

                RotateBoss(-13f);
                MoveToPlayer(6f);
                break;

            case 2:
                // turns only one time per state
                if (isState[2] == false)
                {
                    enemyHealthScr.canTakeDamage = false;
                    AudioManager.Instance.PlaySFX("Explosion");
                    rippleParticle.Play();
                    Debug.Log("state2 @ " + enemyHealthScr.enemyHealth);
                    isState[2] = true;
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
                }

                RotateBoss(10f);
                MoveToPlayer(4f);
                break;
        }
    }

    private void Shooting1()
    {
        foreach (ParticleSystem weapon in particleWeapons)
        {
            weapon.Emit(1);

        }
    }

    private void Shooting2()
    {
        foreach (ParticleSystem weapon in particleWeapons2)
        {
            weapon.Emit(1);
        }
    }

    private void InvokeShootSound()
    {
        AudioManager.Instance.PlaySFX("EnemyShootSound");
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
}