using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string actionMapNameUI = "UI";

    [Header("Action Name References Game")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string rotate = "Rotate";
    [SerializeField] private string boost = "Boost";
    [SerializeField] private string ability = "Ability";
    [SerializeField] private string cameraSwitch = "Camera";
    [SerializeField] private string openUI = "OpenUI";

    private InputAction moveAction;
    private InputAction rotateAction;
    private InputAction boostAction;
    private InputAction abilityAction;
    private InputAction cameraSwitchAction;
    private InputAction openUIAction;

    [Header("Action Name References UI")]
    [SerializeField] private string navigate = "Navigate";
    [SerializeField] private string click = "Click";
    [SerializeField] private string closeUI = "CloseUI";

    private InputAction navigateUIAction;
    private InputAction clickUIAction;
    private InputAction closeUIAction;

    [Header("OtherObjects")]
    public CinemachineSwitcher viewSwitch;

    // Gameplay
    public Vector2 MoveInput { get; private set; }
    public float RotateInput { get; private set; }
    public bool BoostInput { get; private set; }
    public bool AbilityInput { get; private set; }


    public event Action OnCameraSwitchInputChanged;
    private bool _cameraSwitchInput;
    public bool CameraSwitchInput 
    {
        get { return _cameraSwitchInput; }
        private set
        {
            if (_cameraSwitchInput != value)
            {
                _cameraSwitchInput = value;
                if (_cameraSwitchInput)
                {
                    OnCameraSwitchInputChanged?.Invoke();
                }
            }
        }
    }
    
 
    public event Action OnOpenUIChanged;
    private bool _openUIInput;
    public bool OpenUIInput
    {
        get { return _openUIInput; }
        private set
        {
            if (_openUIInput != value)
            {
                _openUIInput = value;
                if (_openUIInput)
                {
                    OnOpenUIChanged?.Invoke();
                }
            }
        }
    }



    // UI Stuff
    public event Action<Vector2> OnNavigateUIInputChanged;
    private Vector2 _navigateUIInput;
    public Vector2 NavigateUIInput
    {
        get { return _navigateUIInput; }
        private set
        {
            if (_navigateUIInput != value)
            {
                _navigateUIInput = value;
                OnNavigateUIInputChanged?.Invoke(_navigateUIInput);
            }
        }
    }

    public event Action OnClickInputChanged;
    private bool _clickUIInput;
    public bool ClickUIInput
    {
        get { return _clickUIInput; }
        private set
        {
            Debug.Log("Klick " + _clickUIInput + " - " + value);
            if (_clickUIInput != value)
            {
                _clickUIInput = value;
                if (_clickUIInput)
                {
                    OnClickInputChanged?.Invoke();
                    _clickUIInput = false;
                }
            }
        }
    }

    public event Action OnCloseUIChanged;
    private bool _closeUIInput;
    public bool CloseUIInput
    {
        get { return _closeUIInput; }
        private set
        {
            if (_closeUIInput != value)
            {
                _closeUIInput = value;
                if (_closeUIInput)
                {
                    OnCloseUIChanged?.Invoke();
                }
            }
        }
    }



    private void Awake()
    {
        // Game Controls
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        rotateAction = playerControls.FindActionMap(actionMapName).FindAction(rotate);
        boostAction = playerControls.FindActionMap(actionMapName).FindAction(boost);
        abilityAction = playerControls.FindActionMap(actionMapName).FindAction(ability);
        cameraSwitchAction = playerControls.FindActionMap(actionMapName).FindAction(cameraSwitch);
        openUIAction = playerControls.FindActionMap(actionMapName).FindAction(openUI);

        // UI Controls
        navigateUIAction = playerControls.FindActionMap(actionMapNameUI).FindAction(navigate);
        clickUIAction = playerControls.FindActionMap(actionMapNameUI).FindAction(click);
        closeUIAction = playerControls.FindActionMap(actionMapNameUI).FindAction(closeUI);
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        // Game Controls
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

        openUIAction.performed += context => OpenUIInput = true;
        openUIAction.canceled += context => OpenUIInput = false;

        // UI Controls
        navigateUIAction.performed += context => NavigateUIInput = context.ReadValue<Vector2>();
        navigateUIAction.canceled += context => NavigateUIInput = Vector2.zero;

        clickUIAction.performed += context => ClickUIInput = true;
        clickUIAction.canceled += context => ClickUIInput = false;

        closeUIAction.performed += context => CloseUIInput = true;
        closeUIAction.canceled += context => CloseUIInput = false;
    }

    private void OnEnable()
    {
        EnableGameConrtols();
    }
    private void OnDisable()
    {
        DisableGameControls();
    }


    public void EnableGameConrtols()
    {
        Debug.Log("1-EnableGameConrtols");
        moveAction.Enable();
        rotateAction.Enable();
        boostAction.Enable();
        abilityAction.Enable();
        cameraSwitchAction.Enable();
        openUIAction.Enable();
    }

    public void DisableGameControls()
    {
        Debug.Log("1-DisableGameControls");
        moveAction.Disable();
        rotateAction.Disable();
        boostAction.Disable();
        abilityAction.Disable();
        cameraSwitchAction.Disable();
        openUIAction.Disable();
    }

    public void EnableUIControls()
    {
        Debug.Log("2-EnableUIControls");
        navigateUIAction.Enable();
        clickUIAction.Enable();
        closeUIAction.Enable();
    }

    public void DisableUIControls()
    {
        Debug.Log("2-DisableUIControls");
        navigateUIAction.Disable();
        clickUIAction.Disable();
        closeUIAction.Disable();
    }
}
