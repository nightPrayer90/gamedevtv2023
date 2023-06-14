using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitControl : MonoBehaviour
{
    public List<ParticleCollisionEvent> collisionEvents; // creating a list to store the collision events
    public Color hitColor;
    public int enemyHealth = 10;

    public GameObject _replacement;
    private float collisionMultiplier = 64;
    private bool broken;

    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.
        /*int damage_ = other.GetComponent<ParticleCollsionScript>().damage;*/
        /*string damageStr = damage_.ToString();

        takeDamage(damage_);*/

        int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
        {
            Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy

            //EffectsManager.DoFloatingText(pos, "+" + damageStr, hitColor);
            //vfx.transform.parent = parentGameobject.transform; // this makes the new gameobjects children to my "VFX Parent" gameObject in my Hierarchy, for organizarion purposes
        }
    }


    public void takeDamage(int damage)
    {
        enemyHealth -= damage;

        //EffectsManager.DoFloatingText(transform.position, "+" + damage, hitColor);

        //destroy
        if (enemyHealth <= 0)
        {
            var replacement = Instantiate(_replacement, transform.position, transform.rotation);
           
            // i dont need this, its only for fun. 
            var rbs = replacement.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                rb.AddExplosionForce(collisionMultiplier, transform.position, 1);  //collision.contacts[0].point;
            }
            
            Destroy(gameObject);
        }

    }

}
