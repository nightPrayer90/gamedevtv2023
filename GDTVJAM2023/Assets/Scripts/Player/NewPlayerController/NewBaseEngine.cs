using UnityEngine;

public class NewBaseEngine : BaseModule
{
    // Objects
    private NewPlayerController playerController;
    private Rigidbody playerRigidbody;
    private GameManager gameManager;
    [HideInInspector] public bool useBoost = false;
    //private bool powerBoostResetFlag = true;

    [Header("Forward Engine")]
    [SerializeField] private ParticleSystem ps_engine;
    public ParticleSystem ps_boostEngine;
    public ParticleSystem ps_boostParticle;
    public float thrustForce = 1f;
    public float totalThrustForce = 0;
    //public bool hasPowerBoost = true;
    public bool hasFontBoost = true;
    public float frontBoostPower;
    public float frontBoostCost = 0.1f;
    //public float powerBoostDowntime = 3f;
    //public float powerBoostCost = 0.3f;
    public ParticleSystem powerBoosParticle;

    [Header("Backwards Engine")]
    [SerializeField] private ParticleSystem[] ps_backEngines;
    [SerializeField] private ParticleSystem[] ps_backBoostEngines;
    public float backForce = 1f;
    private float totalBackForce = 0;
    public bool hasBackBoost = true;
    public float backBoostPower;

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
        // Update Module Values to Player Controller - Function comes from BaseModule
        UpdateModuleValues();

        thrustForce = moduleValues.mainEngine;
        frontBoostPower = moduleValues.boostEngine;

        GameObject go = GameObject.Find("Game Manager");
        gameManager = go.GetComponent<GameManager>();
        playerController = GetComponentInParent<NewPlayerController>();
        playerController.OnIntroOver += HandleStartBoost;
        playerController.OnUpdateRotateSpeed += HandleSpeedUpdate;
        playerRigidbody = playerController.GetComponent<Rigidbody>();
        playerController.hasMainEngine = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShowEnergie();

        // Forwards
        if (playerController.verticalInput > 0.5)
        {
            // boost and abilitys
            if (hasFontBoost == true && (playerController.boostInput || playerController.abilityInput) && playerController.energieCurrent > 0)
            {
                // normal boost
                if (playerController.boostInput && playerController.energieCurrent > 0)
                {
                    totalThrustForce = thrustForce + frontBoostPower;
                    playerController.energieCurrent -= frontBoostCost;

                    totalThrustForce = thrustForce + frontBoostPower;
                    ps_boostParticle.Emit(1);
                    ps_boostEngine.Emit(1);

                    useBoost = true;
                    playerController.useBoost = true;
                }

                if (playerController.energieCurrent < frontBoostCost) useBoost = false;
            }

            // normal Flight
            else
            {
                totalThrustForce = thrustForce;

                useBoost = false;
                playerController.useBoost = false;
            }

            ps_engine.Emit(1);

            // Berechne die Kraft basierend auf dem horizontalen Input
            Vector3 thrust = -transform.right * playerController.verticalInput * totalThrustForce;

            // Wende die Kraft auf das Raumschiff an
            playerRigidbody.AddForce(thrust);
        }

        // Backwards
        if (playerController.verticalInput < -0.5)
        {
            // boost
            if (hasBackBoost == true && playerController.boostInput && (playerController.energieCurrent > 1 || useBoost == true))
            {
                totalBackForce = backForce + backBoostPower;
                foreach (ParticleSystem ps in ps_backBoostEngines)
                {
                    ps.Emit(1);
                }

                playerController.energieCurrent -= frontBoostCost;
                useBoost = true;
                playerController.useBoost = true;

                if (playerController.energieCurrent < frontBoostCost)
                {
                    useBoost = false;
                    playerController.useBoost = false;
                }
            }
            else
            {
                totalBackForce = backForce;
                foreach (ParticleSystem ps in ps_backEngines)
                {
                    ps.Emit(1);
                }
            }

            ps_engine.Emit(1);

            // Berechne die Kraft basierend auf dem horizontalen Input
            Vector3 thrust = -transform.right * playerController.verticalInput * totalBackForce;

            // Wende die Kraft auf das Raumschiff an
            playerRigidbody.AddForce(thrust);
        }
    }

    private void HandleStartBoost()
    {
        ps_engine.Play();
    }

    private void HandleSpeedUpdate(float flyspeed)
    {
        thrustForce = thrustForce * (1 + (flyspeed / 100));
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
}
