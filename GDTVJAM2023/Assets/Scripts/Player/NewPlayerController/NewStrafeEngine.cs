using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewStrafeEngine : BaseModule
{
    private NewPlayerController playerController;
    [SerializeField] private ParticleSystem ps_leftEngine;
    [SerializeField] private ParticleSystem ps_rigtEngine;
    private Rigidbody playerRigidbody;
    public float strafeForce = 1f;
    public float strafeBoostforce = 1;
    private float totalStrafeForce;
    public bool hasStrafeBoost = true;
    [SerializeField] private ParticleSystem ps_leftBoost;
    [SerializeField] private ParticleSystem ps_rigtBoost;
    private bool useBoost = false;
    public float boostCost = 0.1f;
    private int direction = -1; // chance axis 1 or -1

    // Start is called before the first frame update
    void Start()
    {
        // Update Module Values to Player Controller - Function comes from BaseModule
        UpdateModuleValues();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<NewPlayerController>();
        playerRigidbody = playerController.GetComponent<Rigidbody>();

        strafeForce = moduleValues.strafeEngine;
        strafeBoostforce = moduleValues.boostStrafeEngine;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController == null)
            return; // TODO: schön machen

        if (playerController.strafeInput > 0.5)
        {

            // boost
            if (hasStrafeBoost == true && (playerController.energieCurrent > 1 || useBoost == true)) // TODO here was removed: checking strafe input button
            {
                totalStrafeForce = strafeForce + strafeBoostforce;
                ps_leftBoost.Emit(1);

                playerController.energieCurrent -= boostCost;
                useBoost = true;
                playerController.useBoost = true;

                if (playerController.energieCurrent < boostCost) useBoost = false;
            }
            else
            {
                totalStrafeForce = strafeForce * EnergieDebuffForce();
                ps_leftEngine.Emit(1);
                useBoost = false;
                playerController.useBoost = false;
            }

            // Berechne die Kraft basierend auf dem horizontalen Input
            Vector3 thrust = playerRigidbody.transform.right * playerController.strafeInput * -totalStrafeForce * direction;

            // Wende die Kraft auf das Raumschiff an
            playerRigidbody.AddForce(thrust);
        }

        if (playerController.strafeInput < -0.5)
        {
            if (hasStrafeBoost == true && (playerController.energieCurrent > 1 || useBoost == true)) // TODO here was removed: checking strafe input button
            {
                totalStrafeForce = strafeForce + strafeBoostforce;
                ps_rigtBoost.Emit(1);

                playerController.energieCurrent -= boostCost;
                useBoost = true;

                if (playerController.energieCurrent < boostCost)
                {
                    useBoost = false;
                    playerController.useBoost = false;
                }
            }
            else
            {
                totalStrafeForce = strafeForce * EnergieDebuffForce();
                ps_rigtEngine.Emit(1);
                useBoost = false;
                playerController.useBoost = false;
            }

            // Berechne die Kraft basierend auf dem horizontalen Input
            Vector3 thrust = playerRigidbody.transform.right * playerController.strafeInput * -totalStrafeForce * direction;

            // Wende die Kraft auf das Raumschiff an
            playerRigidbody.AddForce(thrust);   
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
