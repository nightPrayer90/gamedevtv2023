using UnityEngine;

public class ParticleBullet : MonoBehaviour
{
    public float bulletDamage = 1.0f;
    //private GameManager gameManager;
    //public ParticleSystem particleSystem;

    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();

    private void Start()
    {
        //gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update()
    {
        /*if (!gameManager.gameIsPlayed || gameManager.gameOver )
        {
            particleSystem.Stop();
        }
        else
        {
            if (particleSystem.isStopped)
                particleSystem.Play();
        }*/
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out EnemyHealth en))
        {
            en.TakeDamage(bulletDamage);
        }
    }



}
