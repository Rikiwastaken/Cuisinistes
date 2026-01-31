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

    public void PlaySFX(AudioClip clip, float pitchrandomness)
    {
        StartCoroutine(PlaySFXCoroutine(clip, pitchrandomness));
    }

    private IEnumerator PlaySFXCoroutine(AudioClip clip, float pitchrandomness)
    {
        GameObject newAudioSource = new GameObject();
        newAudioSource.name = clip.name;
        newAudioSource.transform.parent = SFXHolder;
        AudioSource AS = newAudioSource.AddComponent<AudioSource>();
        AS.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Sound")[0];
        AS.clip = clip;
        AS.volume = SFXVol;
        AS.pitch = 1f + UnityEngine.Random.Range(-pitchrandomness, pitchrandomness);
        AS.Play();
        yield return new WaitForSeconds(AS.clip.length);
        Destroy(newAudioSource);
    }


}
