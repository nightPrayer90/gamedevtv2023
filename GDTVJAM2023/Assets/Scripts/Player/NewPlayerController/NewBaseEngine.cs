using UnityEngine;

public class NewBaseEngine : MonoBehaviour
{
    // Objects
    private NewPlayerController playerController;
    private Rigidbody playerRigidbody;
    private GameManager gameManager;

    [Header("Forward Engine")]
    [SerializeField] private ParticleSystem ps_engine;
    [SerializeField] private ParticleSystem ps_boostEngine;
    [SerializeField] private ParticleSystem ps_boostParticle;
    public float thrustForce = 1f;
    private float totalThrustForce = 0;
    public bool hasPowerBoost = true;
    public bool hasFontBoost = true;
    public float FrontBoostPower;
    public float FrontBoostCost = 0.1f;
    private bool useBoost = false;

    [Header("Backwards Engine")]
    [SerializeField] private ParticleSystem[] ps_backEngines;
    [SerializeField] private ParticleSystem[] ps_backBoostEngines;
    public float backForce = 1f;
    private float totalBackForce = 0;
    public bool hasBackBoost = true;
    public float BackBoostPower;

    [Header("Energie Show Options")]
    public bool canShowEnergie = true;
    public MeshRenderer boostMeshRenderer;
    public Material mat_EnergieFull;
    public Material mat_EnergieEmpty;
    public Material mat_EnergieRegen;
    private int lastMaterial = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<NewPlayerController>();
        playerController.OnIntroOver += HandleStartBoost;
        playerController.OnUpdateRotateSpeed += HandleSpeedUpdate;
        playerRigidbody = playerController.GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShowEnergie();

        // Forwards
        if (playerController.verticalInput > 0.5)
        {
            // boost
            if (hasFontBoost == true && Input.GetButton("Boost") && (playerController.energieCurrent > 1 || useBoost == true))
            {
                totalThrustForce = thrustForce + FrontBoostPower;

                // powerboost
                if (hasPowerBoost == true && playerController.energieCurrent >= playerController.energieMax*0.90f)
                {
                    gameManager.ScreenShake(5);
                    AudioManager.Instance.PlaySFX("PlayerBoostKick");
                    ps_boostParticle.Emit(80);
                    ps_boostEngine.Emit(30);
                    playerRigidbody.AddForce(transform.forward * totalThrustForce * 25, ForceMode.Force);
                    playerController.GetInvulnerability();
                    playerController.energieCurrent -= playerController.energieMax * 0.25f;
                }
                else
                {
                    playerController.energieCurrent -= FrontBoostCost;
                }

                totalThrustForce = thrustForce + FrontBoostPower;
                ps_boostParticle.Emit(1);
                ps_boostEngine.Emit(1);
                
                useBoost = true;
                
                if (playerController.energieCurrent < FrontBoostCost) useBoost = false;
            }
            else
            {
                

                totalThrustForce = thrustForce * EnergieDebuffForce();
                ps_engine.Emit(1);
                useBoost = false;
            }

            // Berechne die Kraft basierend auf dem horizontalen Input
            Vector3 thrust = transform.forward * playerController.verticalInput * totalThrustForce;

            // Wende die Kraft auf das Raumschiff an
            playerRigidbody.AddForce(thrust);

            
        }

        // Backwards
        if (playerController.verticalInput < -0.5)
        {
            // boost
            if (hasBackBoost == true && Input.GetButton("Boost") && (playerController.energieCurrent > 1 || useBoost == true))
            {
                totalBackForce = backForce + BackBoostPower;
                foreach (ParticleSystem ps in ps_backBoostEngines)
                {
                    ps.Emit(1);
                }

                playerController.energieCurrent -= FrontBoostCost;
                useBoost = true;

                if (playerController.energieCurrent < FrontBoostCost) useBoost = false;
            }
            else
            {
                totalBackForce = backForce * EnergieDebuffForce();
                foreach (ParticleSystem ps in ps_backEngines)
                {
                    ps.Emit(1);
                }
            }

            // Berechne die Kraft basierend auf dem horizontalen Input
            Vector3 thrust = transform.forward * playerController.verticalInput * totalBackForce;

            // Wende die Kraft auf das Raumschiff an
            playerRigidbody.AddForce(thrust);

            
        }
    }

    private void HandleStartBoost()
    {
        ps_engine.Play();
        playerRigidbody.AddForce(transform.forward * thrustForce * 160, ForceMode.Force);
    }

    private void HandleSpeedUpdate(float flyspeed)
    {
        thrustForce = thrustForce * (1 + (flyspeed/100));
        backForce = backForce * (1 + (flyspeed / 100));
    }

    private void ShowEnergie()
    {
        int material = 1;

        if (canShowEnergie == true)
        {
            if (playerController.energieCurrent >= playerController.energieMax * 0.90f)
            { 
                material = 1;
            }
            else if (playerController.energieCurrent <= 0.5f)
            {
                material = 2;
            }
            else
            {
                material = 3;
            }

            // nur wenn sich das material geändert hat
            if (lastMaterial != material)
            {
                lastMaterial = material;
                Material[] materials = boostMeshRenderer.materials;

                if (lastMaterial == 1)
                {
                    materials[2] = mat_EnergieFull;
                    ps_boostParticle.Emit(40);
                }
                else if (lastMaterial == 2)
                    materials[2] = mat_EnergieEmpty;
                else if (lastMaterial == 3)
                    materials[2] = mat_EnergieRegen;
                boostMeshRenderer.materials = materials;
            }
        }
    }

    private float EnergieDebuffForce()
    {
        float debuffForce = 1;
        if (playerController.energieProduction < 0)
        {
            debuffForce = 0.7f;
        }
        return debuffForce;
    }
}
