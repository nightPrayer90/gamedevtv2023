using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBaseEngine : MonoBehaviour
{
    private NewPlayerController playerController;
    [SerializeField] private ParticleSystem ps_engine;
    private Rigidbody playerRigidbody;
    public float thrustForce = 1f;


    private enum Side
    {
        forward,
        backward,
        strafingLeft,
        strafingRight
    }

    [SerializeField]
    private Side engine_side;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<NewPlayerController>();
        playerRigidbody = playerController.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Particel Effect
        if (playerController.verticalInput > 0.1)
        {
            if (engine_side == Side.forward)
            {

                // Berechne die Kraft basierend auf dem horizontalen Input
                Vector3 thrust = transform.forward * playerController.verticalInput * thrustForce;


                // Wende die Kraft auf das Raumschiff an
                playerRigidbody.AddForce(thrust);


                ps_engine.Emit(1);
            }
        }

        if (playerController.verticalInput < -0.1)
        {
            if (engine_side == Side.backward)
            {

                // Berechne die Kraft basierend auf dem horizontalen Input
                Vector3 thrust = transform.forward * -playerController.verticalInput * thrustForce;


                // Wende die Kraft auf das Raumschiff an
                playerRigidbody.AddForce(thrust);


                ps_engine.Emit(1);
            }
        }

        if (playerController.horizontalInput2 > 0.1)
        {
            if (engine_side == Side.strafingLeft)
            {

                // Berechne die Kraft basierend auf dem horizontalen Input
                Vector3 thrust = transform.forward * playerController.horizontalInput2 * thrustForce;


                // Wende die Kraft auf das Raumschiff an
                playerRigidbody.AddForce(thrust);


                ps_engine.Emit(1);
            }
        }

        if (playerController.horizontalInput2 < -0.1)
        {
            if (engine_side == Side.strafingRight)
            {

                // Berechne die Kraft basierend auf dem horizontalen Input
                Vector3 thrust = transform.forward * -playerController.horizontalInput2 * thrustForce;


                // Wende die Kraft auf das Raumschiff an
                playerRigidbody.AddForce(thrust);


                ps_engine.Emit(1);
            }
        }
    }
}
