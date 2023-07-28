using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    private Vector3 startTransform;

    // Start is called before the first frame update
    void Start()
    {
        startTransform = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = startTransform;
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
