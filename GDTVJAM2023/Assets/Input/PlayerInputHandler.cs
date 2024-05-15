using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string rotate = "Rotate";
    [SerializeField] private string boost = "Boost";
    [SerializeField] private string ability = "Ability";
    [SerializeField] private string cameraSwitch = "Camera";

    private InputAction moveAction;
    private InputAction rotateAction;
    private InputAction boostAction;
    private InputAction abilityAction;
    private InputAction cameraSwitchAction;

    [Header("OtherObjects")]
    public CinemachineSwitcher viewSwitch;


    public Vector2 MoveInput { get; private set; }
    public float RotateInput { get; private set; }
    public bool BoostInput { get; private set; }
    public bool AbilityInput { get; private set; }

    private bool _cameraSwitchInput;
    public bool CameraSwitchInput 
    {
        get
        {
            return _cameraSwitchInput;
        }
        private set
        {
            // Prüfe, ob der Wert sich ändert
            if (_cameraSwitchInput != value)
            {
                _cameraSwitchInput = value;

                // Wenn CameraSwitchInput auf true gesetzt wird, löse die Methode in einem anderen Objekt aus
                if (_cameraSwitchInput)
                {
                    // Rufe die Methode im anderen Objekt auf
                    viewSwitch.SwitchState();
                }
            }
        }
    }

    private void Awake()
    {
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        rotateAction = playerControls.FindActionMap(actionMapName).FindAction(rotate);
        boostAction = playerControls.FindActionMap(actionMapName).FindAction(boost);
        abilityAction = playerControls.FindActionMap(actionMapName).FindAction(ability);
        cameraSwitchAction = playerControls.FindActionMap(actionMapName).FindAction(cameraSwitch);
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        rotateAction.performed += context => RotateInput = context.ReadValue<float>();
        rotateAction.canceled += context => RotateInput = 0f;

        boostAction.performed += context => BoostInput = true;
        boostAction.canceled += context => BoostInput = false;

        abilityAction.performed += context => AbilityInput = true;
        abilityAction.canceled += context => AbilityInput = false;

        cameraSwitchAction.performed += context => CameraSwitchInput = true;
        cameraSwitchAction.canceled += context => CameraSwitchInput = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        rotateAction.Enable();
        boostAction.Enable();
        abilityAction.Enable();
        cameraSwitchAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        rotateAction.Disable();
        boostAction.Disable();
        abilityAction.Disable();
        cameraSwitchAction.Disable();
    }
}
