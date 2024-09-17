using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopCameraController : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float zoomDamping = 5f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 15f;
    public float dragSpeed = 0.1f;

    private Camera mainCamera;
    private float targetZoomDistance;
    private Vector3 dragStartPosition;

    [Header("Camera Limits")]
    public Vector2 horizontalLimits;
    public Vector2 verticalLimits;

    private bool isMoving = false;
    public bool hasMoveKeys = true;

    void Start()
    {
        mainCamera = Camera.main;
        targetZoomDistance = Vector3.Distance(transform.position, mainCamera.transform.position);
    }

    void Update()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        // Handle mouse drag movement
        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            dragStartPosition = mousePosition;
        }

        if (Mouse.current.middleButton.isPressed)
        {
            Vector3 dragCurrentPosition = mousePosition;
            Vector3 translation = (mainCamera.ScreenToWorldPoint(new Vector3(dragStartPosition.x, dragStartPosition.y, mainCamera.nearClipPlane)) -
                                  mainCamera.ScreenToWorldPoint(new Vector3(dragCurrentPosition.x, dragCurrentPosition.y, mainCamera.nearClipPlane))) * (dragSpeed + (30 + transform.position.y)*3.5f);
            translation.y = 0; // Maintain horizontal plane movement only
            Vector3 newPosition = transform.position + translation;

            // Clamp the new position within the horizontal and vertical limits
            newPosition.x = Mathf.Clamp(newPosition.x, horizontalLimits.x, horizontalLimits.y);
            newPosition.z = Mathf.Clamp(newPosition.z, verticalLimits.x, verticalLimits.y);

            transform.position = newPosition;
            dragStartPosition = dragCurrentPosition;
        }

        // Handle zoom
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            if (scroll > 0 && transform.position.y > minZoomDistance + 5)
            {
                // Zoom in towards the mouse position
                Vector3 direction = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane)) - mainCamera.transform.position;
                direction = direction.normalized;

                float zoomAmount = scroll * zoomSpeed * Time.deltaTime;
                Vector3 targetPosition = transform.position + direction * zoomAmount;

                // Clamp the y position
                targetPosition.y = Mathf.Clamp(targetPosition.y, minZoomDistance, maxZoomDistance);

                // Clamp the x and z positions within the horizontal and vertical limits
                targetPosition.x = Mathf.Clamp(targetPosition.x, horizontalLimits.x, horizontalLimits.y);
                targetPosition.z = Mathf.Clamp(targetPosition.z, verticalLimits.x, verticalLimits.y);

                // Smoothly move the camera parent to the new position
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomDamping);
            }
            else if (scroll < 0 && transform.position.y < maxZoomDistance - 5)
            {
                // Zoom in towards the mouse position
                Vector3 direction = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane)) - mainCamera.transform.position;
                direction = direction.normalized;

                float zoomAmount = scroll * zoomSpeed * Time.deltaTime;
                Vector3 targetPosition = transform.position + direction * zoomAmount;

                // Clamp the y position
                targetPosition.y = Mathf.Clamp(targetPosition.y, minZoomDistance, maxZoomDistance);

                // Clamp the x and z positions within the horizontal and vertical limits
                targetPosition.x = Mathf.Clamp(targetPosition.x, horizontalLimits.x, horizontalLimits.y);
                targetPosition.z = Mathf.Clamp(targetPosition.z, verticalLimits.x, verticalLimits.y);

                // Smoothly move the camera parent to the new position
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomDamping);
            }
        }

        // handle Keys
        if (!isMoving && hasMoveKeys)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(-42, -20, 70)));
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(-32, -20, 70)));
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(-22, -20, 70)));
            }
            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(-12, -20, 70)));
            }
            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(-2, -20, 70)));
            }
            if (Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(8, -20, 70)));
            }
            if (Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(18, -20, 70)));
            }
            if (Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                StartCoroutine(MoveToPosition(new Vector3(28, -20, 70)));
            }
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            isMoving = true;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPos, 15f * Time.deltaTime);

            // Clamp the new position within the horizontal and vertical limits
            newPosition.x = Mathf.Clamp(newPosition.x, horizontalLimits.x, horizontalLimits.y);
            newPosition.z = Mathf.Clamp(newPosition.z, verticalLimits.x, verticalLimits.y);

            transform.position = newPosition;
            yield return null;
            isMoving = false;
        }
    }

}
