using UnityEngine;

public class DropRocketController : RocketController
{
    private Transform player;

    private const string playerTag = "Player";
    private Vector3 target;
    private PlayerController playerController;
    private PlayerWeaponController playerWeaponController;
    private Rigidbody playerRigidBody;

    public float rocketScattering = 1f;
    public GameObject damageMarkerRadius;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void OnEnable()
    {
        InitRocket();

        // set Game Objects
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        playerController = player.GetComponent<PlayerController>();
        playerWeaponController = player.GetComponent<PlayerWeaponController>();
        playerRigidBody = player.GetComponent<Rigidbody>();

        Invoke("DestroyObject", maxLifeTime);

        FindNextTarget(); // only once for this rocket
    }
    private void FixedUpdate()
    {
        // rocket movement
        Vector3 direction = (target - transform.position).normalized;
        transform.position += transform.forward * speed * Time.deltaTime;

        // did we cross play level?
        if (Vector3.Distance(transform.position, target) < 0.3f)
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

    /* **************************************************************************** */
    /* MOVEMENT--------------------------------------------------------------------- */
    /* **************************************************************************** */
    // find and set the next target position
    private void FindNextTarget()
    {
        target = player.position + new Vector3(Random.Range(-rocketScattering, rocketScattering), 0, Random.Range(-rocketScattering, rocketScattering));
        damageMarkerRadius.transform.position = target;
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
            gameManager.DoFloatingText(player.position, "+" + adjustedDamage.ToString(), resultColor);

            // calculate enemy damage
            playerController.UpdatePlayerHealth(adjustedDamage);
            playerRigidBody.AddExplosionForce(explosionForce, pos, explosionRadius);

            // calculate shield damage
            bool soundFlag = false;
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
            }
        }

        // spawn the explosion object
        GameObject go = ObjectPoolManager.SpawnObject(exposionHitObject, pos, transform.rotation, ObjectPoolManager.PoolType.ParticleSystem);

        go.GetComponent<ParticleSystemDestroy>().rippleParicleSize = explosionRadius;

        // object goes back to the pool
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}

