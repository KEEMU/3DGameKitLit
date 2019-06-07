using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereAttack : MonoBehaviour
{
    public static SphereAttack instance;
    public Transform m_MuzzlePosition;

    public enum WeaponType
    {
        Seeker
    }

    [Header("Seeker")]
    public Transform seekerProjectile;
    public Transform seekerMuzzle;
    public Transform seekerImpact;
    public float vulcanClipDelay;
    public AudioClip vulcanShot;

    private float timer;
    private AudioSource sphereAudioSource;

    private void Awake()
    {
        instance = this;
    }
    private void Seeker()
    {
        PoolManager.instance.RequestMuzzle(WeaponType.Seeker, seekerMuzzle,m_MuzzlePosition.position, m_MuzzlePosition.rotation);
        PlaySpwanAudio();
        PoolManager.instance.RequestProjectile(WeaponType.Seeker, seekerProjectile, m_MuzzlePosition.position, m_MuzzlePosition.rotation);

    }
    private void PlaySpwanAudio()
    {

    }
    public void SeekerImpact(Vector3 pos)
    {
        PoolManager.instance.RequestImpact(WeaponType.Seeker, seekerImpact, pos, Quaternion.identity);
        //PlaySeekerImpactAudio();
        
    }
    private void PlaySeekerImpactAudio()
    {
        if (timer >= vulcanClipDelay)
        {
            sphereAudioSource.clip = vulcanShot;
            sphereAudioSource.pitch = Random.Range(0.95f, 1f);
            sphereAudioSource.volume = Random.Range(0.8f, 1f);
            sphereAudioSource.minDistance = 5f;
            sphereAudioSource.loop = false;
            sphereAudioSource.Play();
            timer = 0f;
        }
        
    }
    void Start()
    {
        sphereAudioSource = GetComponent<AudioSource>();
    }
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
            Seeker();
    }
}
