using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HangarManager : MonoBehaviour
{
    public GameObject playerStartPoint;
    public GameObject playerHangarShip;
    public List<Color> classColors;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 positionOffset = new Vector3(0f, 0.05f, 0f);
        playerHangarShip.transform.position = playerStartPoint.transform.position + positionOffset;
    }


}
