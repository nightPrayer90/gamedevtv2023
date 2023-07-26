using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarManager : MonoBehaviour
{
    public GameObject playerStartPoint;
    public GameObject playerHangarShip;

    // Start is called before the first frame update
    void Start()
    {
        playerHangarShip.transform.position = playerStartPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
