using DG.Tweening;
using UnityEngine;

public class HangarRotateShips : MonoBehaviour
{
    private int rotationState = 0; // Aktueller Zustand der Rotation (0, 90, 180, 270 Grad)

    private void Start()
    {
        UpdateRotationState(); // Aktualisiere die Rotation entsprechend dem aktuellen Zustand
    }

    public void RotateShip( int rotateIndex)
    {
        switch (rotateIndex)
        {
            case 1:
                rotationState = 0;
                break;
            case 2:
                rotationState = 90;
                break;
            case 3:
                rotationState = 180;
                break;
            case 4:
                rotationState = 270;
                break; 
        }
        UpdateRotationState();
        Debug.Log(rotationState);
    }

    private void UpdateRotationState()
    {
        // Drehe das Objekt zur entsprechenden Rotation mit DOTween
        transform.DORotate(new Vector3(0f, rotationState, 0f), 1f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => Debug.Log("Rotation completed"));
    }
}
