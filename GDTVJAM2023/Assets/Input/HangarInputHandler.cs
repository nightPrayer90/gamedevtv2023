using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HangarInputHandler : MonoBehaviour, _9Dystricts.IHangarActions
{
    public event Action OnModuleRemove;
    public event Action<Vector2> OnNavigatePress;
    public event Action<float> OnPresetChange;
    private _9Dystricts inputWrapper;
    private bool isNavigating;
    private Vector2 naviDirection;

    private void Start()
    {
        inputWrapper = new();
        inputWrapper.Hangar.AddCallbacks(this);
        inputWrapper.Enable();
    }

    private void OnDestroy()
    {
        inputWrapper.Hangar.RemoveCallbacks(this);
        inputWrapper.Disable();
    }

    public void OnAbort(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnGameStart(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            isNavigating = false;
            StopAllCoroutines();
            return;
        }

        Vector2 value = context.ReadValue<Vector2>();
        
        if(context.performed)
        {
            naviDirection.x = CustomRound(value.x, 0.32f); // sign of 0 is 0 for C# Sign
            naviDirection.y = CustomRound(value.y, 0.32f);
            if (naviDirection.magnitude > 0 && !isNavigating)
            {
                isNavigating = true;
                IEnumerator coroutine = IntervalSendNavigate(0.28f);
                StartCoroutine(coroutine);
            }
        }
    }

    private int CustomRound(float value, float threshold)
    {
        return (Mathf.Abs(value) >= threshold ? Math.Sign(value) : 0);
    }

    private IEnumerator IntervalSendNavigate(float waitTime)
    {
        while(isNavigating)
        {
            OnNavigatePress?.Invoke(naviDirection);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void OnRemove(InputAction.CallbackContext context)
    {   
        if(context.performed) OnModuleRemove?.Invoke();
    }

    public void OnRemoveUnconnected(InputAction.CallbackContext context)
    {
        if (context.performed)  Debug.Log("hold");
        //throw new System.NotImplementedException();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSwitchArea(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnPreset(InputAction.CallbackContext context)
    {
        if (context.performed) OnPresetChange?.Invoke(context.ReadValue<float>());
    }
}
