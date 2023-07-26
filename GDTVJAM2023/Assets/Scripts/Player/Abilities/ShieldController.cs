using UnityEngine;
using DG.Tweening;

public class ShieldController : MonoBehaviour
{
    private Transform playerTr;
    private PlayerWeaponController playerWeaponController;
    private GameManager gameManager;
    private Rigidbody playerRb;
    private FrontShieldSpawner frontShieldSpawner;
    private BackShieldSpawner backShieldSpawner;

    public Material targetMaterial;

    public int shieldLife = 10;
    private int shieldLife_;
    public bool isBackShield = false;
    public bool isBackShieldLeft = false;
    public bool isBackShieldRight = false;

    public ParticleSystem dieEffect;
    public ParticleSystem hitEffect;
    public GameObject shieldMesh;
    public Vector3 shieldSize;
    public BoxCollider shieldCollider;

    private bool canGetDamage = true;


    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        // get player Components
        var player = GameObject.FindWithTag("Player");
        playerTr = player.GetComponent<Transform>();
        playerRb = player.GetComponent<Rigidbody>();
        playerWeaponController = player.GetComponent<PlayerWeaponController>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        shieldSize = shieldMesh.transform.localScale;

        ShieldEnable();
        targetMaterial.DOFade(0, 0.01f);
    }

    // set position to player position
    private void LateUpdate()
    {
        if (playerTr != null)
        {
            transform.position = playerTr.position;
            transform.rotation = playerTr.rotation;
        }
    }

    // if collision with an Enemy
    private void OnCollisionEnter(Collision collision)
    {
        // enemy target tag set
        string tagStr = "Enemy";
        if (gameManager.dimensionShift == true)
        {
            tagStr = "secondDimensionEnemy";
        }

        // shield destroy on collosion and gives the player a force in the backwards direction
        if (collision.gameObject.CompareTag(tagStr))
        {
            if (tagStr == "Enemy")
            {
                // player bounce
                Vector3 explosionDirection = collision.transform.position - transform.position;
                explosionDirection.Normalize();
                playerRb.AddForce(explosionDirection * -1f * .1f, ForceMode.Impulse);

                // enemy bounce
                Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
                enemyRb.AddForce(explosionDirection * 1f * 10f, ForceMode.Impulse);

                hitEffect.Play();

                // update shield health
                UpdateShieldHealth();
            }
            else
            {
                // player bounce
                Vector3 explosionDirection = collision.transform.position - transform.position;
                explosionDirection.Normalize();
                playerRb.AddForce(explosionDirection * -1f * 10f, ForceMode.Impulse);

                // destroy shild
                ShieldTypeDieControl();
            }
        }
    }


    /* **************************************************************************** */
    /* SHIELD MANAGEMENT----------------------------------------------------------- */
    /* **************************************************************************** */
    // manuel OnEnable Event for the shield object
    public void ShieldEnable()
    {
        AudioManager.Instance.PlaySFX("ShieldActivate");
        shieldMesh.transform.DOPunchScale(new Vector3(1f, 1f, 1f), 1f, 5, 0.5f).SetUpdate(true);

        // reset shild life
        canGetDamage = true;
        shieldLife_ = shieldLife;
        
        // set shield status
        if (isBackShield == false)
        {
            playerWeaponController.isFrontShieldEnabled = true;
            frontShieldSpawner = GameObject.Find("front Shield").GetComponent<FrontShieldSpawner>();
        
        }
        else
        {
            backShieldSpawner = GameObject.Find("back Shield").GetComponent<BackShieldSpawner>();
            if (isBackShieldLeft == true)
            {
                playerWeaponController.isBackShieldLeft = isBackShieldLeft;
            }
            else
            {
                playerWeaponController.isBackShieldRight = isBackShieldRight;
            }
        }

        // set material Color
        shieldCollider.enabled = true;
        shieldMesh.SetActive(true);

        targetMaterial.DOFade(0.8f, 1f);
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.
        var ps = other.GetComponent<EnemyParticleBullet>();

        if (ps != null)
            UpdateShieldHealth();
    }

    // the shield get an hit from a bullet!
    public void UpdateShieldHealth()
    {
        // Update shield life
        if (canGetDamage == true)
        {
            shieldLife_ = Mathf.Max(0, shieldLife_ - 1);
            canGetDamage = false;
            Invoke("CanDamageControl", 0.4f);

            // destroy shield
            if (shieldLife_ <= 0)
            {
                ShieldTypeDieControl();
                AudioManager.Instance.PlaySFX("ShieldDie");
            }
            else
            {
                // update shield color;
                shieldMesh.transform.DOKill();
                shieldMesh.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.3f, 5, 0.5f).OnComplete(() => { shieldMesh.transform.localScale = shieldSize; } );
                UpdateShildColor();
                AudioManager.Instance.PlaySFX("ShieldGetHit");
            }
        }
     }

    private void CanDamageControl()
    {
        canGetDamage = true;
    }

    // Update the weapon and shieldcontroller if the ShildObject die
    private void ShieldTypeDieControl()
    {
        // update all Controller
        if (isBackShield == false)
        {
            // front shild
            playerWeaponController.isFrontShieldEnabled = false;
            frontShieldSpawner.SpawnFrondShieldControl();
        }
        else
        {
            if (isBackShieldLeft == true)
            {
                playerWeaponController.isBackShieldLeft = false;
                backShieldSpawner.SpawnBackShildLeftControl();
            }
            else
            {
                playerWeaponController.isBackShieldRight = false;
                backShieldSpawner.SpawnBackShildRightControl();
            }
        }

        // set the shild die Effect
        dieEffect.Play();

        // deactivate the shield Object
        targetMaterial.DOFade(0f, 0.01f);
        shieldCollider.enabled = false;
        shieldMesh.SetActive(false);
    }

    // update the alpha value from the shield material 
    private void UpdateShildColor()
    {
        float newAlpha = 1f - (0.9f / shieldLife_);

        targetMaterial.DOKill();
        targetMaterial.DOFade(newAlpha, 0.5f);
    }
}