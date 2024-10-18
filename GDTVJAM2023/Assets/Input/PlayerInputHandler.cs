using System;
using Unity.VisualScripting;
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
    [SerializeField] private string openMap = "OpenMap";
    [SerializeField] private string hideUI = "HideUI";

    private InputAction moveAction;
    private InputAction rotateAction;
    private InputAction boostAction;
    private InputAction abilityAction;
    private InputAction cameraSwitchAction;
    private InputAction openUIAction;
    private InputAction openMapAction;
    private InputAction hideUIAction;
    

    [Header("Action Name References UI")]
    [SerializeField] private string navigate = "Navigate";
    [SerializeField] private string click = "Click";
    [SerializeField] private string closeUI = "CloseUI";
    [SerializeField] private string reroll = "Reroll";
    [SerializeField] private string stats = "Stats";

    private InputAction navigateUIAction;
    private InputAction clickUIAction;
    private InputAction closeUIAction;
    private InputAction rerollAction;
    private InputAction statsAction;

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


    public event Action OnOpenMapInputChanged;
    private bool _openMapInput;
    public bool OpenMapInput
    {
        get { return _openMapInput; }
        private set
        {
            if (_openMapInput != value)
            {
                _openMapInput = value;
                OnOpenMapInputChanged?.Invoke();
            }
        }
    }


    public event Action OnHideUI;
    private bool _hideUI;
    public bool HideUI
    {
        get { return _hideUI; }
        private set
        {
            _hideUI = value;
            if (_hideUI)
            {
                OnHideUI?.Invoke();
            }
        }
    }





    // UI Stuff ------------------------------------------------------------------
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

    public event Action OnReroll;
    private bool _rerollInput;
    public bool RerollInput
    {
        get { return _rerollInput; }
        private set
        {
            if (_rerollInput != value)
            {
                _rerollInput = value;
                if (_rerollInput)
                {
                    OnReroll?.Invoke();
                }
            }
        }
    }

    public event Action OnStats;
    private bool _statsInput;
    public bool StatsInput
    {
        get { return _statsInput; }
        private set
        {
            if (_statsInput != value)
            {
                _statsInput = value;
                if (_statsInput)
                {
                    OnStats?.Invoke();
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
        openMapAction = playerControls.FindActionMap(actionMapName).FindAction(openMap);
        hideUIAction = playerControls.FindActionMap(actionMapName).FindAction(hideUI);

        // UI Controls
        navigateUIAction = playerControls.FindActionMap(actionMapNameUI).FindAction(navigate);
        clickUIAction = playerControls.FindActionMap(actionMapNameUI).FindAction(click);
        closeUIAction = playerControls.FindActionMap(actionMapNameUI).FindAction(closeUI);
        rerollAction = playerControls.FindActionMap(actionMapNameUI).FindAction(reroll);
        statsAction = playerControls.FindActionMap(actionMapNameUI).FindAction(stats);
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

        openMapAction.performed += context => OpenMapInput = true;
        openMapAction.canceled += context => OpenMapInput = false;

        hideUIAction.performed += context => HideUI = true;
        hideUIAction.canceled += context => HideUI = false;


        // UI Controls
        navigateUIAction.performed += context => NavigateUIInput = context.ReadValue<Vector2>();
        navigateUIAction.canceled += context => NavigateUIInput = Vector2.zero;

        clickUIAction.performed += context => ClickUIInput = true;
        clickUIAction.canceled += context => ClickUIInput = false;

        closeUIAction.performed += context => CloseUIInput = true;
        closeUIAction.canceled += context => CloseUIInput = false;

        rerollAction.performed += context => RerollInput = true;
        rerollAction.canceled += context => RerollInput = false;

        statsAction.performed += context => StatsInput = true;
        statsAction.canceled += context => StatsInput = false;
    }

    private void OnEnable()
    {
        EnableGameControls();
    }
    private void OnDisable()
    {
        DisableGameControls();
    }


    public void EnableGameControls()
    {
        Debug.Log("1-EnableGameControls");
        moveAction.Enable();
        rotateAction.Enable();
        boostAction.Enable();
        abilityAction.Enable();
        cameraSwitchAction.Enable();
        openUIAction.Enable();
        openMapAction.Enable();
        hideUIAction.Enable();
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
        openMapAction.Disable();
        hideUIAction.Disable();
    }

    public void EnableUIControls()
    {
        Debug.Log("2-EnableUIControls");
        navigateUIAction.Enable();
        clickUIAction.Enable();
        closeUIAction.Enable();
        rerollAction.Enable();
        statsAction.Enable();
    }

    public void DisableUIControls()
    {
        Debug.Log("2-DisableUIControls");
        navigateUIAction.Disable();
        clickUIAction.Disable();
        closeUIAction.Disable();
        rerollAction.Disable();
        statsAction.Disable();
    }
}
