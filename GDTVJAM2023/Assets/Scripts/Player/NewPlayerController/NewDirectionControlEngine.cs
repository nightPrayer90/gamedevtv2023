using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDirectionControlEngine : MonoBehaviour
{
    private enum Side
    {
        left,
        right
    }

    private NewPlayerController playerController;
    [SerializeField] private ParticleSystem ps_engine;
    private Rigidbody playerRigidbody;
    public float torqueForce = 1f;
    private Vector3 engineUPPos;

    [SerializeField]
    private Side engine_side;




    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<NewPlayerController>();
        if (playerController != null) {
            playerController.OnUpdateRotateSpeed += HandleRotateSpeed;
            playerRigidbody = playerController.GetComponent<Rigidbody>();
            engineUPPos = transform.up;

            // Change the center of Mass
            //playerRigidbody.centerOfMass = playerRigidbody.centerOfMass + new Vector3(transform.localPosition.x / 2, 0f, 0f);// transform.localPosition.z/2);

            float z_Position = Mathf.Sign(playerRigidbody.centerOfMass.z + transform.localPosition.z);
            float x_Position = Mathf.Sign(playerRigidbody.centerOfMass.x + transform.localPosition.x);           
            engine_side = (z_Position == -1) ? Side.right : Side.left;
            int engineRotation = Mathf.RoundToInt(transform.localRotation.eulerAngles.y);
            Debug.Log(engineRotation);

            switch (engineRotation) {
                case 90:
                    if (x_Position == -1) {
                        engineUPPos = -transform.up;
                    }
                    if (x_Position == 1) {
                        engineUPPos = transform.up;
                    }
                    break;

                case 180:
                    if (z_Position == -1) {
                        engineUPPos = transform.up;
                    }
                    if (z_Position == 1) {
                        engineUPPos = -transform.up;
                    }
                    break;

                case 270:
                    if (x_Position == -1) {
                        engineUPPos = transform.up;
                    }
                    if (x_Position == 1) {
                        engineUPPos = -transform.up;
                    }
                    break;
                case 0:
                    if (z_Position == -1) {
                        engineUPPos = -transform.up;
                    }
                    if (z_Position == 1) {
                        engineUPPos = transform.up;
                    }
                    break;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController != null) {
            if (playerController.horizontalInput > 0.1) {
                if (engine_side == Side.right) {
                    Vector3 torque = engineUPPos * playerController.horizontalInput * torqueForce;

                    playerRigidbody.AddTorque(torque);

                    ps_engine.Emit(1);
                }
            }

            if (playerController.horizontalInput < -0.1) {
                if (engine_side == Side.left) {
                    Vector3 torque = engineUPPos * -playerController.horizontalInput * torqueForce;

                    playerRigidbody.AddTorque(torque);

                    ps_engine.Emit(1);
                }
            }
        }
    }

    private void HandleRotateSpeed(float RotateSpeed)
    {
        torqueForce = torqueForce * (1 + (RotateSpeed/100));
    }
}
