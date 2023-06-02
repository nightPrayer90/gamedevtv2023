using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("Particle Settings")]
    public int damage = 5;
    public float firerateBetweenASalve = 1f;
    public bool isFireSlave = true;
    public float salveFirerate = 0.5f;
    public int salveCount = 5;
    [HideInInspector] public bool isOnFire = false;


    [Header("Particsystem summon?")]
    public bool isSummon = false;

    [Header("How much Systems get Summen? - if isSummon = true")]
    public int numberOfNewPS;
    
    [Range(0f, 360f)]
    public float angleRange = 360f;
    [Tooltip("Corrects the spread in the direction of view if Range < 360°")]
    public bool isAdjustedAngle= false;
    private float angle;

    [Header("Particle Settings - if isSummon = true")]
    public Material material;
    public Color color;
    public float speed;
    public float lifetime;
    public float size;
    public bool isTextureModule = false;
    public Sprite texture;
    public bool isTrail = false;
    public Material trailMaterial;
    public Color trailColorOverLifetime;
    public Color colorOverTrail;
    private AnimationCurve trailCurve = new AnimationCurve();
    [Range(0f,1f)] public float trailStartWidth;
    [Range(0f, 1f)] public float trailEndWidth;
    [Range(0f, 1f)] public float trailEndWidthPosition;
    public bool isSizeOverLivetime;
    private AnimationCurve sizeCurve = new AnimationCurve();
    [Range(0f, 1f)] public float sizePositonToGetSmall;


    //Sonstige
    [HideInInspector]public ParticleSystem system;
    private float timer;
    private int countEmission = 0;
    private float emissionTimer = 0;

    // Crates "number_of_columns" Particle Systems
    void Summon()
    {

        angle = (angleRange / numberOfNewPS);
        float adjustedAngle = 0;
        if (isAdjustedAngle == true)
            adjustedAngle = angleRange / 2 - angle / 2; 

        for (int i = 0; i < numberOfNewPS; i++)
        {
            // A simple particle material with no texture.
            Material particleMaterial = material;

            // Create a Particle System that shoots one particle
            var go = new GameObject("Particle System");
            go.transform.Rotate(0, (angle * i)- adjustedAngle, 0); // Rotate so the system emits upwards.
            go.transform.parent = this.transform;
            go.transform.position = this.transform.position;
            system = go.AddComponent<ParticleSystem>();
            //go.AddComponent<ParticleCollsionScript>();
            go.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
            
            
            // Particle System
            var mainModule = system.main;
            mainModule.startColor = color;
            mainModule.startSize = size;
            mainModule.startSpeed = speed;
            mainModule.startLifetime = lifetime;
            mainModule.maxParticles = 10000;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

            // Emission Modul
            var emission = system.emission;
            emission.enabled = false;

            // Shape Modul
            var forma = system.shape;
            forma.enabled = true;
            forma.shapeType = ParticleSystemShapeType.Sprite;
            forma.sprite = null;

            //Textrue Sheet Animation Module
            if (isTextureModule == true)
            {
                var text = system.textureSheetAnimation;
                text.enabled = true;
                text.mode = ParticleSystemAnimationMode.Sprites;
                text.AddSprite(texture);
            }

            // Collision Module
            var coll = system.collision;
            coll.enabled = true;
            coll.lifetimeLoss = 1;
            coll.type = ParticleSystemCollisionType.World;
            coll.sendCollisionMessages = true;

            // Trails Modul
            if (isTrail == true)
            {
                var trail = system.trails;
                trail.enabled = true;
                Material trailMaterial_ = trailMaterial;
                go.GetComponent<ParticleSystemRenderer>().trailMaterial = trailMaterial_;
                trail.colorOverLifetime = trailColorOverLifetime;
                trail.colorOverTrail = colorOverTrail;
                trailCurve.AddKey(0.0f, trailStartWidth);
                trailCurve.AddKey(trailEndWidthPosition, trailEndWidth);
                trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1.0f, trailCurve);
            }

            //Size over Livetime Modul
            if(isSizeOverLivetime)
            {
                var sizeOverLF = system.sizeOverLifetime;
                sizeOverLF.enabled = true;
                sizeCurve.AddKey(sizePositonToGetSmall, 1.0f);
                sizeCurve.AddKey(1.0f, 0.0f);
                sizeOverLF.size = new ParticleSystem.MinMaxCurve(1.0f, sizeCurve);
            }
        }
    }

    // Löst das Emitieren einen Partikels aller ChildPartikelsystme aus
    void DoEmit()
    {
        foreach (Transform child in transform)
        {
            system = child.GetComponent<ParticleSystem>();            
            system.Emit(1);
        }
    }

    // Löscht alle Childsysteme wenn diese erzeugt wurden
    private void DestroyChilds()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            SingleShot();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isOnFire)
            {
                StartEmission();
            }
            else
            {
                StopEmission();
            }
        }

        if (isOnFire == true)
        {
            timer += Time.deltaTime;

            if (!isFireSlave)
                SingleEmission();
            else
                SalveEmission();
        }
    }

    // StartFireing
    public void StartEmission()
    {
        if (isSummon == true) Summon();
        isOnFire = true;
        timer = firerateBetweenASalve;
        emissionTimer = salveFirerate;
        countEmission = 0;
    }

    //StopFireing
    public void StopEmission()
    {
        isOnFire = false;
        if (isSummon == true) DestroyChilds();
    }

    // Fire only one single Bullet
    private void SingleEmission()
    {
        // Überprüfen, ob die Spawnzeit erreicht ist
        if (timer >= firerateBetweenASalve)
        {
            DoEmit();

            timer = 0f;
        }
    }

    // Fire a Salve of bullets
    private void SalveEmission()
    {
        if (timer >= firerateBetweenASalve)
        {
            emissionTimer += Time.deltaTime;

            if (emissionTimer >= salveFirerate)
            {
                DoEmit();

                countEmission++;
                emissionTimer = 0f;
            }

            if (countEmission >= salveCount)
            {
                timer = 0f; // reset timer
                countEmission = 0; // reset counter
            }
        }
    }

    // Löst einen einzelnen Schuss aus
    public void SingleShot()
    {
        DoEmit();
    }
}

 