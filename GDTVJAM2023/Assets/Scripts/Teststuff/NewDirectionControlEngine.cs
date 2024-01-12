using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDirectionControlEngine : MonoBehaviour
{
    private NewPlayerController playerController;
    [SerializeField] private ParticleSystem ps_engine;
    private Rigidbody playerRigidbody;
    public float torqueForce = 1f;

    private enum Side
    {
        left,
        right
    }


    [SerializeField]
    private Side engine_side;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<NewPlayerController>();
        playerRigidbody = playerController.GetComponent<Rigidbody>();

        // Chance the center of Mass
        //playerRigidbody.centerOfMass = playerRigidbody.centerOfMass + new Vector3(transform.localPosition.x/2, 0f, transform.localPosition.z/2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController.horizontalInput > 0.1)
        {
            if (engine_side == Side.left)
            {
                // Berechne das Drehmoment basierend auf der seitlichen Eingabe
                //Vector3 torque = new Vector3(0f, playerController.horizontalInput, 0f) * torqueForce;
                // Holen Sie sich die lokale Up-Achse des Triebwerks
                
                // to do - muss nicht zur laufzeit passieren
                float z_Position = Mathf.Sign(playerRigidbody.centerOfMass.z + transform.localPosition.z);
                float x_Position = Mathf.Sign(playerRigidbody.centerOfMass.x + transform.localPosition.x);
                Vector3 engineUPPos = transform.up;
                int engineRotation = Mathf.RoundToInt(transform.localRotation.eulerAngles.y);

                switch (engineRotation)
                {
                    case 0:
                        //Unterhalb des schwerpunktes
                        if (x_Position == -1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = -transform.up;
                        }
                        if (x_Position == 1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = transform.up;
                        }
                        break;

                    case 90:
                        //Unterhalb des schwerpunktes
                        if (z_Position == -1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = transform.up;
                        }
                        if (z_Position == 1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = -transform.up;
                        }
                        break;

                    case 180:
                        //Unterhalb des schwerpunktes
                        if (x_Position == -1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = transform.up;
                        }
                        if (x_Position == 1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = -transform.up;
                        }
                        break;
                    case 270:
                        //Unterhalb des schwerpunktes
                        if (z_Position == -1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = -transform.up;
                        }
                        if (z_Position == 1)
                        {
                            //Vector3 engineUp = (transform.localRotation.y - 0.5f) * transform.up;
                            engineUPPos = transform.up;
                        }
                        break;

                }







                // Berechne die Drehkraft basierend auf der seitlichen Eingabe und der Kraftrichtung
                Vector3 torque = engineUPPos * playerController.horizontalInput * torqueForce;

              
                //Debug.Log("engineUP " + engineUp);
                Debug.Log("engineUPpos " + engineUPPos);

                Debug.Log("z_Position " + z_Position + " / x_Position " + x_Position);
       


                // Debug.Log("localRotation " + transform.localRotation.y);*/


                // Berechne die Drehkraft basierend auf der seitlichen Eingabe und der Up-Achse des Triebwerks
                //Vector3 torque = engineUp * playerController.horizontalInput * torqueForce;

                // Wende die Drehkraft auf das Rigidbody an
                playerRigidbody.AddTorque(torque);


                ps_engine.Emit(1);
            }
        }

        if (playerController.horizontalInput < -0.1)
        {
            if (engine_side == Side.right)
            {
                // Berechne das Drehmoment basierend auf der seitlichen Eingabe
                //Vector3 torque = new Vector3(0f, playerController.horizontalInput, 0f) * torqueForce;

                // Holen Sie sich die lokale Up-Achse des Triebwerks
                Vector3 engineUp = transform.up;

                // Berechne die Drehkraft basierend auf der seitlichen Eingabe und der Up-Achse des Triebwerks
                Vector3 torque = engineUp * playerController.horizontalInput * torqueForce;

                // Wende die Drehkraft auf das Rigidbody an
                playerRigidbody.AddTorque(torque);


                ps_engine.Emit(1);
            }
        }
    }
}
