using UnityEngine;
using System.Collections;
public class EnemyParticleBullet : MonoBehaviour
{
    public int bulletDamage = 0;
    public ParticleSystem bulletParticleSystem;

    public bool canReflect = false;

    //List<ParticleCollisionEvent> colEvents = new List<ParticleCollisionEvent>();
    public void Start()
    {
        bulletParticleSystem = GetComponent<ParticleSystem>();
    }


    public void HardBulletStop()
    {
        bulletParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    /*[System.Obsolete]
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("reflect");
        // Partikel kollidiert mit einem Collider (z.B. eine Wand)

        ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];
        int numCollisionEvents = bulletParticleSystem.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            // Erhalte die Kontakt-Normalenrichtung
            Vector3 contactNormal = collisionEvents[i].normal;
            Debug.Log(contactNormal);

            // Verwende die Kontakt-Normalenrichtung, um das Partikelverhalten anzupassen
            // z.B. indem du den reflektierten Richtungsvektor für das Partikelsystem berechnest.
        }
    }

    */
}
