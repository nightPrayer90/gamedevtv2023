using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalLaser : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 10f;

    private void Update()
    {
        // Erhalten Sie die aktuelle lokale Rotation des Objekts
        Quaternion currentRotation = transform.localRotation;

        // Berechnen Sie die neue Rotation basierend auf dem Rotationsspeed
        Quaternion newRotation = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f) * currentRotation;

        // Setzen Sie die neue Rotation als lokale Rotation des Objekts
        transform.localRotation = newRotation;
    }
}
