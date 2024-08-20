using UnityEngine;
//Simple class to let a object explode over time

[RequireComponent(typeof(Explosion))]
public class DamageExplosionController : MonoBehaviour
{
    private Explosion explosion;
    public float timeToExplode = 0.5f;
    public int damage = 0;
    public int force = 0;
    public float radius = 0;
    public int novaOnDieTriggerType = -1;

    private void Awake()
    {
        explosion = gameObject.GetComponent<Explosion>();
    }

    private void OnEnable()
    {
        Invoke(nameof(Explode), timeToExplode);
    }

    private void Explode()
    {
        explosion.InitExplosion(damage, force, radius, false, null, novaOnDieTriggerType);
    }
}
