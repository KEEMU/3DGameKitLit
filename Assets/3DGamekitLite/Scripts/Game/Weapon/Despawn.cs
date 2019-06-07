using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float DespawnDelay; // Despawn delay in ms
    public bool DespawnOnMouseUp; // Despawn on mouse up used for beams demo 
    public AudioClip audioClip;
    private float timer;
    AudioSource aSrc; // Cached audio source component

    void Awake()
    {
        // Get audio source component
        
        aSrc = GetComponent<AudioSource>();
    }

    // OnSpawned called by pool manager 
    public void OnSpawned()
    {
        PlayAudio();
        //// Invokes despawn using timer delay
        //if (!DespawnOnMouseUp)
        //    F3DTime.time.AddTimer(DespawnDelay, 1, DespawnOnTimer);
    }
    void PlayAudio()
    {
        aSrc.clip = audioClip;
        aSrc.pitch = Random.Range(0.8f, 1f);
        aSrc.volume = Random.Range(0.8f, 1f);
        aSrc.minDistance = 25f;
        aSrc.loop = false;
        aSrc.Play();
    }
    // OnDespawned called by pool manager 
    public void OnDespawned()
    {

    }

    // Run required checks for the looping audio source and despawn the game object
    public void DespawnOnTimer()
    {
        if (aSrc != null)
        {
            if (aSrc.loop)
                DespawnOnMouseUp = true;
            else
            {
                DespawnOnMouseUp = false;
                _Despawn();
            }
        }
        else
        {
            _Despawn();
        }
    }

    // Despawn game object this script attached to
    public void _Despawn()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= DespawnDelay)
        {
            _Despawn();
            timer = 0;
        }
            

    }
}
