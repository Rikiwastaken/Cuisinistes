using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    public Transform SFXHolder;
    public AudioMixer AudioMixer;
    public float SFXVol;
    public float maxdistancetonullify;

    [Serializable]
    public class SoundEffectClass
    {
        public int objectID;
        public AudioClip GrabSFX;
        public AudioClip CrashSFX;
    }

    public List<SoundEffectClass> GrabSFXList;


    private void Awake()
    {
        instance = this;
    }

    public void PlaySFX(AudioClip clip, float pitchrandomness, Transform Emiter)
    {
        StartCoroutine(PlaySFXCoroutine(clip, pitchrandomness, Emiter));
    }

    private IEnumerator PlaySFXCoroutine(AudioClip clip, float pitchrandomness, Transform Emiter)
    {
        GameObject newAudioSource = new GameObject();
        newAudioSource.name = clip.name;
        newAudioSource.transform.parent = SFXHolder;
        newAudioSource.transform.position = Emiter.position;
        IndividualSoundScript ISS = newAudioSource.AddComponent<IndividualSoundScript>();
        ISS.basevolume = SFXVol;
        ISS.Emiter = Emiter;
        ISS.PlayerTransform = MovementController.instance.transform;
        ISS.maxdistancetonullify = maxdistancetonullify;
        AudioSource AS = newAudioSource.AddComponent<AudioSource>();
        AS.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Sound")[0];
        AS.clip = clip;



        Vector3 playerposition = MovementController.instance.transform.position;

        float distance = Vector3.Distance(playerposition, Emiter.position);

        float volume = 0;

        if (distance < maxdistancetonullify)
        {
            volume = (maxdistancetonullify - distance) / maxdistancetonullify;
        }

        AS.volume = SFXVol * volume;
        AS.pitch = 1f + UnityEngine.Random.Range(-pitchrandomness, pitchrandomness);
        AS.Play();
        yield return new WaitForSeconds(AS.clip.length);
        Destroy(newAudioSource);
    }


}
