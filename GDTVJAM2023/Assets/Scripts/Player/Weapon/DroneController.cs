using UnityEngine;
using System;

public class DroneController : MonoBehaviour
{
    // Zustände der Drohne
    private enum DroneState { Orbit, FollowPlayer, AttackEnemy, ReturnToPlayer }

    public Transform player;
    public float followDistance = 5f;
    public float maxDistanceToPlayer = 15f;
    public float moveSpeed = 5f;
    public float orbitSpeed = 30f;
    public float attackDistance = 2f;
    public float forceMultiplier = 10f;
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;
    public float rotationSpeed = 2f;

    private Rigidbody rb;
    private float currentAngle;
    private Transform currentTarget;
    private GameManager gameManager;

    private DroneState currentState;
    private bool canAttack = true;

    public ParticleSystem mainThruster;
    public ParticleSystem leftThruster;
    public ParticleSystem rightThruster;

    private Quaternion previousRotation;

    // Events
    public event Action TriggerAttack;

    public ParticleSystem baseAttackPS;
    private bool isBaseAttack = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentAngle = UnityEngine.Random.Range(0f, 360f);
        player = GameObject.Find("NewPlayer").GetComponent<Transform>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // Startzustand setzen
        currentState = DroneState.Orbit;

        previousRotation = transform.rotation;

        InvokeRepeating(nameof(BaseAttack), 1f, 0.5f);
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        // Y-Achse der Drohne an die Y-Achse des Spielers anpassen
        Vector3 currentPosition = transform.position;
        currentPosition.y = player.position.y;
        transform.position = currentPosition;

        // Aktuellen Zustand der Drohne verarbeiten
        switch (currentState)
        {
            case DroneState.Orbit:
                isBaseAttack = false;
                OrbitAroundPlayer();
                SearchForEnemies();
                break;

            case DroneState.FollowPlayer:
                isBaseAttack = false;
                FollowPlayer();
                SearchForEnemies();
                break;

            case DroneState.AttackEnemy:
                isBaseAttack = true;
                AttackEnemyBehavior();
                break;

            case DroneState.ReturnToPlayer:
                isBaseAttack = false;
                ReturnToPlayer();
                break;
        }

        UpdateThrusterParticles();
        previousRotation = transform.rotation;
    }

    private void SearchForEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        if (enemies.Length > 0)
        {
            currentTarget = enemies[0].transform;
            currentState = DroneState.AttackEnemy;
        }
    }

    private void OrbitAroundPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > maxDistanceToPlayer)
        {
            currentState = DroneState.ReturnToPlayer;
            return;
        }

        currentAngle += orbitSpeed * Time.fixedDeltaTime;
        currentAngle %= 360;

        Vector3 offset = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0, Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * followDistance;
        Vector3 targetPosition = player.position + offset;
        Vector3 direction = (targetPosition - transform.position).normalized;

        rb.AddForce(direction * moveSpeed * forceMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);

        // Physikbasierte Rotation zur Flugrichtung
        ApplyRotationTowards(rb.velocity.normalized);
    }

    private void FollowPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        rb.AddForce(directionToPlayer * moveSpeed * forceMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);

        ApplyRotationTowards(directionToPlayer);

        if (Vector3.Distance(transform.position, player.position) <= followDistance)
        {
            currentState = DroneState.Orbit;
        }
    }

    private void AttackEnemyBehavior()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            currentState = DroneState.Orbit;
            return;
        }

        Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
        rb.AddForce(directionToTarget * moveSpeed * forceMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);

        ApplyRotationTowards(directionToTarget);

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget < attackDistance)
        {
            AttackEnemy();
            Vector3 avoidDirection = (transform.position - currentTarget.position).normalized;
            rb.AddForce(avoidDirection * moveSpeed * forceMultiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        if (distanceToTarget > detectionRadius)
        {
            currentTarget = null;
        }

        if (Vector3.Distance(transform.position, player.position) > maxDistanceToPlayer)
        {
            currentState = DroneState.ReturnToPlayer;
        }
    }

    private void BaseAttack()
    {
        if (isBaseAttack == true)
        {
            baseAttackPS.Emit(1);
            Vector3 recoilDirection = -transform.forward;

            // Anwenden der Rückstoßkraft auf das Rigidbody der Drohne
            rb.AddForce(recoilDirection * 0.015f, ForceMode.Impulse);
        }
    }

    private void ReturnToPlayer()
    {
        FollowPlayer();
        if (Vector3.Distance(transform.position, player.position) <= followDistance)
        {
            currentState = DroneState.Orbit;
        }
    }

    // Physikbasierte Rotation anwenden
    private void ApplyRotationTowards(Vector3 targetDirection)
    {
        if (targetDirection == Vector3.zero) return;

        // Bestimme die Zielrotation basierend auf der Richtung
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));

        // Berechne die notwendige Winkelgeschwindigkeit zur Zielrotation
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        // Anwenden der Rotationskraft proportional zum Winkel und der Zeit
        rb.AddTorque(axis * angle * rotationSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    // Thruster Control
    private void UpdateThrusterParticles()
    {
        // Force
        if (rb.velocity.magnitude > 0.5)
        {
            mainThruster.Emit(1);
        }

        // Torsion
        if (rb.angularVelocity.y > 0.1)
        {
            leftThruster.Emit(1);
        }
        else if (rb.angularVelocity.y < -0.1)
        {
            rightThruster.Emit(1);
        }
    }

    // Placeholder für die Angriffsfunktion
    private void AttackEnemy()
    {
        if (canAttack == true)
        {
            // Angriffscode hier hinzufügen
            Invoke(nameof(ResetAttack), 2f);
            TriggerAttack?.Invoke();
            canAttack = false;
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}