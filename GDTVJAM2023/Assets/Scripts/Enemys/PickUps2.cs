using UnityEngine;
using System;

public class PickUps2 : MonoBehaviour
{
    public float detectRange = 0.5f;
    public int pickupTyp = 0; 
    // 0 = Upgradepickup
    // 1 = Dimensionpickup
    // 2 = Victorypickup
    
    private bool ifcollect = false;
    public ParticleSystem OrbParticle;

    [HideInInspector] public NewPlayerController playerController;
    private Transform playerTransform;
    [HideInInspector] public GameManager gameManager;


    void OnEnable()
    {
        var player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerTransform = player.GetComponent<Transform>();
            playerController = player.GetComponent<NewPlayerController>();
        }
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        ifcollect = false;
        OrbParticle.gameObject.SetActive(true);

        //InvokeRepeating(nameof(DetectPlayer), 5f, 0.01f);
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(playerTransform.position, gameObject.transform.position);
    


        if (distanceToPlayer <= detectRange)
        {
            if (ifcollect == false)
            {
                OrbParticle.gameObject.SetActive(false);
                CancelInvoke();

                playerController.CollectUpdatePickup(pickupTyp);

                ifcollect = true;
                Destroy(gameObject);
            }
        }
    }
}
