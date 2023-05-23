using UnityEngine;

public class test : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public float xMin, xMax, zMin, zMax;
    public float yValue;

    private ParticleSystem.Particle[] particles;

    private void Start()
    {
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }

    private void LateUpdate()
    {
        int particleCount = particleSystem.GetParticles(particles);

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 position = particles[i].position;

            // Begrenze die Position auf der X- und Z-Achse
            //position.x = Mathf.Clamp(position.x, xMin, xMax);
            //position.z = Mathf.Clamp(position.z, zMin, zMax);

            // Setze das Y auf den gewünschten Wert
            position.y = yValue;

            particles[i].position = position;
        }

        particleSystem.SetParticles(particles, particleCount);
    }
}