using UnityEngine;

public class AllyController : MonoBehaviour
{
    private GameManager gameManager;
    private Transform playerTransform;

    public float allyRange = 2f;
    public int timerTime = 15;
    private float timerSteps;

    public ParticleSystem orbitParicle;
    public ParticleSystem timerParicle;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerTransform = GameObject.Find("NewPlayer").transform;

        timerSteps = 360 / timerTime;
        ResetTimer();
    }

    private void FixedUpdate()
    {
        if (gameManager.dimensionShift == false && orbitParicle.isPlaying == false)
        {
            orbitParicle.Play();
        }
        if (gameManager.dimensionShift == true && orbitParicle.isPlaying == true)
        {
            orbitParicle.Stop();
        }

        if (gameManager.dimensionShift == false && TriggerDistance() == true)
        {
            Debug.Log("player in ally radius");
            ResetTimer();
        }
    }

    private bool TriggerDistance()
    {
        bool distanceCheck = false;

        // Berechne den quadratischen Abstand zwischen dem Spieler und dem anderen Objekt
        float distance = Vector3.Distance(playerTransform.position, gameObject.transform.position);

        // Optional: Vergleiche mit einem anderen quadratischen Abstand
        if (distance < allyRange)
        {
            distanceCheck = true;
        }
        return distanceCheck;
    }


    private void ResetTimer()
    {
        // Hole das Shape Module des Particle Systems
        var shape = timerParicle.shape;

        // Überprüfe, ob der Shape-Typ "Circle" ist
        if (shape.shapeType == ParticleSystemShapeType.Circle)
        {
            shape.arc = 0;
        }

        Invoke(nameof(InvokeTimer),1f);
    }

    private void InvokeTimer()
    {
        // Hole das Shape Module des Particle Systems
        var shape = timerParicle.shape;


        Debug.Log("Invoke");
        // Überprüfe, ob der Shape-Typ "Circle" ist
        if (shape.shapeType == ParticleSystemShapeType.Circle)
        {
            shape.arc = Mathf.Min(shape.arc + timerSteps,360);

            Debug.Log(shape.arc);
        }
    }
}
