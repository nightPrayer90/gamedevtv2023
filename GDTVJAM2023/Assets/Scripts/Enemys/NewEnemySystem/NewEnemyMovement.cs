using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class NewEnemyMovement : MonoBehaviour
{
    private Enemy enemy;

    public enum MoveTyp
    {
        BasicFollow,
        DashFollow,
        CircleFollw,
        ZickZack,
        GroundUnit
    }

    [Header("Movement Typ")]
    public MoveTyp moveTyp;
    public bool canTilt = false;
    public float tiltBoarder = 20;

    [Header("Movement Settings")]
    public float speed;
    public float rotationSpeed;



    [Header("Dash Settings")]
    public float dashForce = 10f;
    public float dashDistance = 5f;
    private bool isDash = false;
    public ParticleSystem dashParticle;


    [Header("Rotate Settings")]
    public float radius = 5f; // Radius um den Spieler herum
    public float rotationSpeedRange = 1f; // Streuung der Rotationsgeschwindigkeit
    public float radiusRange = 1f; // Streuung des Radius


    [Header("ZickZack Settings")]
    public float frequency = 1f;
    public float amplitude = 0.5f;
    public float frequencyRange = 1f;
    public float amplitudeRange = 0.5f;


    //GameObjects
    private Rigidbody enemyRb;
    private EnemyBullet enemyBullet;

    private float previousYRotation;
    float tiltAngle = 0;

    /* **************************************************************************** */
    /* LIFECYCLE METHODEN---------------------------------------------------------- */
    /* **************************************************************************** */
    private void Awake()
    {
        // randomize values
        radius += Random.Range(-radiusRange, radiusRange);
        rotationSpeed += Random.Range(-rotationSpeedRange, rotationSpeedRange);

        frequency += Random.Range(-frequencyRange, frequencyRange);
        amplitude += Random.Range(-amplitudeRange, amplitudeRange);

        enemy = gameObject.GetComponent<Enemy>();
        enemyBullet = gameObject.GetComponent<EnemyBullet>();
        enemyRb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        tiltAngle = 0;
        previousYRotation = 0;
    }

    void Update()
    {
        if (enemy.isOutOfDimension == false)
        {
            // set state
            if (enemy.distanceToPlayer >= 10)
            {
                EnemyMovement(canTilt);
                // if (enemyBullet != null) enemyBullet.StopShooting(); // TODO
            }
            else
            {
                switch (moveTyp)
                {
                    case MoveTyp.BasicFollow:
                        EnemyMovement(canTilt);
                        break;

                    case MoveTyp.DashFollow:
                        EnemyMovementWithDash();
                        break;

                    case MoveTyp.CircleFollw:
                        EnemeyMovementCircle();
                        break;

                    case MoveTyp.ZickZack:
                        EnemyMovementZickZack();
                        break;
                }
                // if (enemyBullet != null) enemyBullet.StartShooting(); // TODO
            }
        }
    }



    /* **************************************************************************** */
    /* Movment -------------------------------------------------------------------- */
    /* **************************************************************************** */


    // movment - basic follow the Player
    void EnemyMovement(bool canTilt = false)
    {
        Vector3 directionToPlayer = enemy.playerRb.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        targetRotation *= Quaternion.Euler(0f, 180f, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 movement = directionToPlayer.normalized * speed * Time.deltaTime;
        transform.position += movement;

        TiltMesh(canTilt);
    }

    private void TiltMesh(bool canTilt = false)
    {
        if (canTilt == true)
        {
            float currentYRotation = transform.rotation.eulerAngles.y;
            float angleDifference = Mathf.DeltaAngle(currentYRotation, previousYRotation);
            tiltAngle = Mathf.Lerp(tiltAngle, angleDifference, 1f * Time.deltaTime);
            float resultAngle = Mathf.Clamp(-tiltAngle * 60f, -tiltBoarder, tiltBoarder);
            enemy.enemyMesh.transform.localRotation = Quaternion.Euler(0, 0, resultAngle);
            previousYRotation = currentYRotation;
        }
    }

    // movment - with dash  - Mass 2 - Drag 2 - force - 6.5 distance 2.7
    void EnemyMovementWithDash()
    {
        Vector3 playerPosition = enemy.playerRb.transform.position;
        Vector3 directionToPlayer = playerPosition - transform.position;
        directionToPlayer.y = 0f; // Nur in der horizontalen Ebene

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            targetRotation *= Quaternion.Euler(0f, 180f, 0f);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (enemy.enemyMesh != null)
            {
                Vector3 forwardDirection = directionToPlayer.normalized;
                forwardDirection.y = 0f;
                float tiltAngle = Mathf.Clamp(Vector3.Angle(forwardDirection, Vector3.forward) - 90f, 0f, 15f);
                enemy.enemyMesh.transform.rotation = Quaternion.Euler(tiltAngle - 90, targetRotation.eulerAngles.y + 90, targetRotation.eulerAngles.z);
            }
        }

        if (enemy.distanceToPlayer < dashDistance && isDash == false)
        {
            // Dash
            Vector3 dashDirection = (enemy.playerRb.transform.position - transform.position).normalized;
            Rigidbody enemyRb = GetComponent<Rigidbody>();
            enemyRb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            dashParticle.Play();
            isDash = true;
        }
        else
        {
            if (isDash == false)
            {
                Vector3 movement = directionToPlayer.normalized * speed * Time.deltaTime;
                transform.position += movement;
            }
            else
            {
                if (enemyRb.velocity.magnitude <= 0.5)
                {
                    isDash = false;
                }
            }
        }
    }

    // movment - enemy flys around the player
    private void EnemeyMovementCircle()
    {
        Vector3 targetDirection = enemy.playerRb.position - transform.position;
        targetDirection.y = 0f; // Nur in der horizontalen Ebene drehen
        Quaternion targetRotation = Quaternion.LookRotation(-targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 targetPosition = enemy.playerRb.position + (transform.right * radius);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    // movment - enemy do strange stuff - f = 0.5 a = 10 rf = 0.2 ar2
    void EnemyMovementZickZack()
    {
        Vector3 playerPosition = enemy.playerRb.transform.position;
        Vector3 directionToPlayer = playerPosition - transform.position;
        directionToPlayer.y = 0f; // Nur in der horizontalen Ebene

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            targetRotation *= Quaternion.Euler(0f, 180f, 0f);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 leftOffset = transform.right * -1f;
        Vector3 rightOffset = transform.right;
        float zickZackOffset = Mathf.PingPong(Time.time * frequency, 1f) * amplitude;

        Vector3 targetPosition;
        if (Mathf.FloorToInt(Time.time * frequency) % 2 == 0)
        {
            targetPosition = playerPosition + leftOffset + transform.forward * zickZackOffset;
        }
        else
        {
            targetPosition = playerPosition + rightOffset + transform.forward * zickZackOffset;
        }

        Vector3 movement = (targetPosition - transform.position).normalized * speed * Time.deltaTime;
        transform.position += movement;
    }
}
