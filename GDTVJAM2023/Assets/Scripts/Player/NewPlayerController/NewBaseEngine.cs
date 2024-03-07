using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBaseEngine : MonoBehaviour
{
    // Objects
    private NewPlayerController playerController;
    private Rigidbody playerRigidbody;

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


    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<NewPlayerController>();
        playerRigidbody = playerController.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
                    AudioManager.Instance.PlaySFX("PlayerBoostKick");
                    ps_boostParticle.Emit(80);
                    ps_boostEngine.Emit(30);
                    playerRigidbody.AddForce(transform.forward * totalThrustForce * 20, ForceMode.Force);
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
                totalThrustForce = thrustForce;
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
                totalBackForce = backForce;
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

    public void StartBoost()
    {
        ps_engine.Play();
        playerRigidbody.AddForce(transform.forward * thrustForce * 80, ForceMode.Force);
    }
}
