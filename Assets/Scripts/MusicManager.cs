using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{

    public Transform SFXHolder;
    public AudioMixer AudioMixer;
    public float SFXVol;

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
        AS.pitch = 1f + Random.Range(-pitchrandomness, pitchrandomness);
        yield return new WaitForSeconds(AS.clip.length);
        Destroy(newAudioSource);
    }


}
