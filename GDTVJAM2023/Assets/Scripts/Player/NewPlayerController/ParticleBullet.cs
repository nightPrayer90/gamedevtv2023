using UnityEngine;

public class ParticleBullet : MonoBehaviour
{
    public int bulletDamage = 0;
    public int damageTyp = 0; // 0= bullet - 1= Laser
    public void BulletSetDamage(int bulletDamage_)
    {
        bulletDamage = bulletDamage_;
    } 
}
