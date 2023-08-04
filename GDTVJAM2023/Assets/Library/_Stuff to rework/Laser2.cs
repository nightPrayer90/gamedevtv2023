using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser2 : MonoBehaviour
{
    public int maxBounces = 5;
    private LineRenderer lr;
    public Transform startPoint;
    public float laserDistance = 10;

    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, startPoint.position);
        InvokeRepeating("InvokeTest", 0.1f, 0.02f);

        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void InvokeTest()
    {
        Debug.Log("test");
        //player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        CastLaser(startPoint.transform.position, -startPoint.transform.forward);
    }


    void CastLaser(Vector3 position, Vector3 direction)
    {
        lr.SetPosition(0, startPoint.position);

        for (int i = 0; i< maxBounces; i++)
        {
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, laserDistance, 1)) //TODOO LAyermask
            {
                position = hit.point;
                direction = Vector3.Reflect(direction, hit.normal);
                lr.SetPosition(i+1, hit.point);

                
                if (hit.transform.tag != "Enemy")
                {
                    for (int j=( i+1); j <= maxBounces; j++)
                    {
                        lr.SetPosition(j, hit.point);
                    }

                    if (hit.transform.tag == "Player")
                    {

                        //player.GetLaserHit();

                    }
                    break;
                }
                else
                {
                    lr.SetPosition(i + 1, hit.point);
                }
            }
            else
            {
                lr.SetPosition(i+1, ray.GetPoint(laserDistance));
            }
        }
    }
}
