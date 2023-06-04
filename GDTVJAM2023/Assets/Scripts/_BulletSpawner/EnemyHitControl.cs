using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitControl : MonoBehaviour
{
    public List<ParticleCollisionEvent> collisionEvents; // creating a list to store the collision events
    public Color hitColor;

    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>(); // *** important! Making a variable to acess the particle system of the emmiting object, in this case, the lasers from my player ship.
        //string damageStr = other.GetComponent<ParticleCollsionScript>().damage.ToString();


        int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in collisionEvents) //  for each collision, do the following:
        {
            Vector3 pos = collisionEvent.intersection; // the point of intersection between the particle and the enemy

            //EffectsManager.DoFloatingText(pos, "+" + damageStr, hitColor);
            //vfx.transform.parent = parentGameobject.transform; // this makes the new gameobjects children to my "VFX Parent" gameObject in my Hierarchy, for organizarion purposes
        }
    }

}
