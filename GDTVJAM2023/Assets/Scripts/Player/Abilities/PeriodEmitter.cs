using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodEmitter : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public float spawnInterval = 2f;
    private float nextSpawnTime = 0f;

    public bool isAudioEmitter=false;
    private bool isPlaying = false;
    public string audioClip = "";
    public float audioClipPlayDuration = 0f;
    public float audioClipDuration = 0f;
    private float nextAudioClipPlay = 0f;
    private float audioPlayTime = 0f;

    private void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Play();
            }

            isPlaying = true;
            audioPlayTime = Time.time + audioClipPlayDuration;
            nextSpawnTime = Time.time + spawnInterval;
        }

        if (isAudioEmitter == true && isPlaying == true)
        {
            if (Time.time <= audioPlayTime)
            {
                //Hier wird der sound abgespielt
                if (Time.time >= nextAudioClipPlay)
                {
                    AudioManager.Instance.PlaySFX(audioClip);
                    nextAudioClipPlay = Time.time + audioClipDuration;

                }
                
            }
            else
            {
                isPlaying = false;
            }
        }
    }
}
