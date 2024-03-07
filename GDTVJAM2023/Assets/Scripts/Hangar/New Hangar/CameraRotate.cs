using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float MouseSpeed = 3;
    [SerializeField] float orbitDampig = 10;

    Vector3 localRot;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;

        if (Input.GetMouseButton(2))
        {

            localRot.x += Input.GetAxis("Mouse Y") * MouseSpeed;
            localRot.y += Input.GetAxis("Mouse X") * MouseSpeed;

            localRot.x = Mathf.Clamp(localRot.y, 0f, 0);

            Quaternion QT = Quaternion.Euler(localRot.x, localRot.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, QT, Time.deltaTime * orbitDampig);
        }
    }
}
