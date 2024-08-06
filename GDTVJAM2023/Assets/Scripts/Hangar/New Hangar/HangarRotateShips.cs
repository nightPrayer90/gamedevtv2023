using DG.Tweening;
using UnityEngine;

public class HangarRotateShips : MonoBehaviour
{
    public int rotationState = 0; // Aktueller Zustand der Rotation (0, 90, 180, 270 Grad)
    public NavigationPanelManager btn1;
    public NavigationPanelManager btn2;
    public NavigationPanelManager btn3;
    public NavigationPanelManager btn4;

    public HangarShipTransform shipTransform;

    public HangarInputHandler hangarInputHandler;
    public ModuleStorage moduleStorage;

    private void Start()
    {
        hangarInputHandler.OnPresetChange += HangarInputHandler_OnPresetChange;
    }

    private void HangarInputHandler_OnPresetChange(float value)
    {
        Debug.Log(value);
        int index = rotationState / 90 + 1;
        index += (int)value;
        if (index > 4) index = 1;
        if (index < 1) index = 4;
        RotateShip(index, true);

        switch (index)
        {
            case 1:
                btn1.ChangeShip(0);
                break;
            case 2:
                btn2.ChangeShip(1); 
                break;
            case 3:
                btn3.ChangeShip(2); 
                break;
            case 4:
                btn4.ChangeShip(3); 
                break;
        }
    }

    public void RotateShip( int rotateIndex, bool isSmooth)
    {

        Debug.Log("rotate index " + rotateIndex);

        // TODO: make it smarter
        btn1.nameText.color = Color.white;
        btn2.nameText.color = Color.white;
        btn3.nameText.color = Color.white;
        btn4.nameText.color = Color.white;

        shipTransform.TweenShip();

        switch (rotateIndex)
        {
            case 1:
                rotationState = 0;
                btn1.ChangeBtnColor();
                break;
            case 2:
                rotationState = 90;
                btn2.ChangeBtnColor();
                break;
            case 3:
                rotationState = 180;
                btn3.ChangeBtnColor();
                break;
            case 4:
                rotationState = 270;
                btn4.ChangeBtnColor();
                break; 
        }

        UpdateRotationState(isSmooth);
    }

    private void UpdateRotationState(bool isSmooth)
    {
        if (isSmooth == true)
        {
            // Drehe das Objekt zur entsprechenden Rotation mit DOTween
            transform.DOKill();
            transform.DORotate(new Vector3(0f, rotationState, 0f), 1f)
                .SetEase(Ease.InOutQuad);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, rotationState, 0f);
        }
    }
}
