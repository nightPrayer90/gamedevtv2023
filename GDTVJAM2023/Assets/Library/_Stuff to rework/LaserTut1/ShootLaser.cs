using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    public Material laserMaterial;
    LaserBeam beam;



    void Start()
    {
        //Destroy(GameObject.Find("LaserBeam"));
        beam = new LaserBeam(gameObject.transform.position, gameObject.transform.right, laserMaterial);
    }
}
