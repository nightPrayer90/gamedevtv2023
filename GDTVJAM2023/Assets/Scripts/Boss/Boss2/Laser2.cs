using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser2 : MonoBehaviour
{
    public int maxBounces = 5;
    private LineRenderer lr;
    public float laserDistance = 10;
    public int laserDamage = 3;

    private PlayerController player;
    private GameManager gameManager;
    private string tagStr;
    private LayerMask layerMask;

    public ParticleSystem muzzleParticle;
    public ParticleSystem hitParticle;

    public bool isShootOnEnable = false;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        lr = GetComponent<LineRenderer>();
        //lr.SetPosition(0, transform.position);

        // reset all points
        for (int i = 0; i <= maxBounces; i++)
        {
            lr.SetPosition(i, transform.position);
        }
    }

    private void OnEnable()
    {
        GameObject playerOb = GameObject.FindWithTag("Player");
        if (playerOb != null)
        {
            player = playerOb.GetComponent<PlayerController>();
        }

        muzzleParticle.Play();

        if (isShootOnEnable == true)
        {
            LaserActivate();
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            Debug.Log("playerget");
        }
    }


    public void LaserActivate()
    {
        tagStr = "Enemy";
        if (gameManager.dimensionShift == true) { tagStr = "secondDimensionEnemy"; }
        layerMask = (1 << 6) | (1 << 7) | (1<< 8) | (1 << 9);

        InvokeRepeating("InvokeShoot", 0.01f, 0.02f);
    }


    public void LaserStop()
    {
        CancelInvoke("InvokeShoot");
        gameObject.SetActive(false);
        muzzleParticle.Stop();
        hitParticle.Stop();
    }

    void InvokeShoot()
    {
        CastLaser(transform.position, transform.forward);
    }


    void CastLaser(Vector3 position, Vector3 direction)
    {

        float remainingLaserDistance = laserDistance;

        lr.SetPosition(0, transform.position);

        for (int i = 0; i< maxBounces; i++)
        {
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, remainingLaserDistance, layerMask))
            {
                position = hit.point;
                direction = Vector3.Reflect(direction, hit.normal);
                lr.SetPosition(i+1, hit.point);

                float distance = Vector3.Distance(lr.GetPosition(i), hit.point);
                remainingLaserDistance = laserDistance - distance;

                if (hit.transform.tag != tagStr)
                {
                    for (int j=( i+1); j <= maxBounces; j++)
                    {
                        lr.SetPosition(j, hit.point);
                    }

                    if (hit.transform.tag == "Player")
                    {
                        hitParticle.transform.position = hit.point;
                        hitParticle.Emit(2);
                        player.GetLaserDamage(laserDamage);
                    }

                    if (hit.transform.tag == "PlayerLaser")
                    {
                        OrbitalLaserOrb orb = hit.transform.GetComponent<OrbitalLaserOrb>();

                        hitParticle.transform.position = hit.point;
                        hitParticle.Emit(2);
                        orb.DestroyOrb();
                    }

                    if (hit.transform.tag == "Shield")
                    {
                        ShieldController shield = hit.transform.GetComponent<ShieldController>();

                        hitParticle.transform.position = hit.point;
                        hitParticle.Emit(2);
                        shield.UpdateShieldHealth(1);
                    }

                    break;
                }
                else
                {
                    lr.SetPosition(i + 1, hit.point);
                    hitParticle.transform.position = hit.point;
                    hitParticle.Emit(2);
                }
            }
            else
            {
                lr.SetPosition(i+1, ray.GetPoint(remainingLaserDistance));
            }
        }
    }
}
