using UnityEngine;

public class DropRocketController : MonoBehaviour
{
    [Header("Rocket Settings")]
    [HideInInspector] public int damage; // rocked damage
    public float speed = 10f; // rocked Speed
    public float rotationSpeed = 5f; // rotation speed
    public float startTime = 0.5f; // time until the rocked flys to a target
    protected float rotationSpeedtmp = 0;
    public GameObject exposionHitObject;
    public float iniLifeTime;
    private float maxLifeTime;   // time before the rocked get destroyed
    [HideInInspector] public Color hitColor;
    public bool isMainWeapon = false;

    [Header("Explosion Control")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    private LayerMask layerMask;


    [Header("Game Objects")]
    public GameObject trail;
    protected GameManager gameManager;
    private UpgradeChooseList upgradeChooseList;
    protected PlayerWeaponController playerWeaponController;


    [Header("Enemy RocketController")]
    public float rocketScattering = 1f;
    public GameObject damageMarkerRadius;
    private Transform player;

    private const string playerTag = "Player";
    private Vector3 targetV3;
    private NewPlayerController playerController;
    private Rigidbody playerRigidBody;

    public int damageZoneChance = 30;
    public GameObject dropPrefab;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void OnEnable()
    {
        InitRocket();

        // set Game Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        playerController = player.GetComponent<NewPlayerController>();
        playerWeaponController = player.GetComponent<PlayerWeaponController>();
        playerRigidBody = player.GetComponent<Rigidbody>();

        maxLifeTime = iniLifeTime; 
        Invoke("DestroyObject", maxLifeTime);

        FindNextTarget(); // only once for this rocket
    }

    private void InitRocket()
    {
        rotationSpeedtmp = rotationSpeed;
        rotationSpeed = 0;
        Invoke(nameof(SetRotateSpeed), startTime);

        if (trail != null)
            Invoke(nameof(ActivateTrail), 0.2f);
    }

    private void FixedUpdate()
    {
        // rocket movement
        Vector3 direction = (targetV3 - transform.position).normalized;
        transform.position += transform.forward * speed * Time.deltaTime;

        // did we cross play level?
        if (Vector3.Distance(transform.position, targetV3) < 0.3f)
        {
            //rotationSpeed = 0; // fall down
            Explode();
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag) || other.gameObject.CompareTag("Shield"))
        {
            Explode();
        }
    }

    protected void OnDisable()
    {
        if (trail != null)
            trail.SetActive(false);
    }

    /* **************************************************************************** */
    /* MOVEMENT--------------------------------------------------------------------- */
    /* **************************************************************************** */
    // find and set the next target position
    private void FindNextTarget()
    {
        targetV3 = player.position + new Vector3(Random.Range(-rocketScattering, rocketScattering), 0, Random.Range(-rocketScattering, rocketScattering));
        damageMarkerRadius.transform.position = targetV3;
        damageMarkerRadius.SetActive(true);
    }

    // set the rotatespeed and a new target after start
    private void SetRotateSpeed()
    {
        rotationSpeed = rotationSpeedtmp;
    }

    /* **************************************************************************** */
    /* FUNCTIONS TO RUN------------------------------------------------------------ */
    /* **************************************************************************** */
    // destroy function 
    private void DestroyObject()
    {
        Explode();
    }

    private void Explode()
    {
        // postion of explosion Object
        Vector3 pos = transform.position;

        // cancel invoke
        CancelInvoke("DestroyObject");
        
        // calculate distance between explosioncenter and objects in Range
        float distance = Vector3.Distance(pos, player.position);

        Color resultColor = hitColor;
        int adjustedDamage = 0;

        if (distance < explosionRadius)
        {
            float scaleFactor = Mathf.Min(1.4f - (distance / explosionRadius), 1f);
            adjustedDamage = Mathf.CeilToInt(damage * scaleFactor);

            // show floating text
            gameManager.DoFloatingText(player.position, adjustedDamage.ToString(), resultColor);

            // calculate enemy damage
            playerController.UpdatePlayerHealth(adjustedDamage);
            playerRigidBody.AddExplosionForce(explosionForce, pos, explosionRadius);

            // calculate shield damage
            /*bool soundFlag = false;
            if (playerWeaponController.isFrontShieldEnabled == true)
            {
                playerWeaponController.frontShield_.GetComponent<ShieldController>().UpdateShieldHealth(2);
                soundFlag = true;
            }
            if (playerWeaponController.isBackShieldLeft == true)
            {
                playerWeaponController.backShieldLeft_.GetComponent<ShieldController>().UpdateShieldHealth(2);
                soundFlag = true;
            }
            if (playerWeaponController.isBackShieldRight == true)
            {
                playerWeaponController.backShieldRight_.GetComponent<ShieldController>().UpdateShieldHealth(2);
                soundFlag = true;
            }
            if (soundFlag == true)
            {
                AudioManager.Instance.PlaySFX("ShieldGetHit");
            }*/
        }

        // spawn drop Object
        if (dropPrefab != null && Random.Range(0,100) < damageZoneChance)
        {
            Instantiate(dropPrefab, pos, exposionHitObject.transform.rotation);
        }


        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(exposionHitObject, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;

        // object goes back to the pool
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void ActivateTrail()
    {
        if (trail != null)
            trail.SetActive(true);
    }

}

