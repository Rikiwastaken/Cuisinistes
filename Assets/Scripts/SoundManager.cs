using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    public Transform SFXHolder;
    public AudioMixer AudioMixer;
    public float SFXVol;
    public float maxdistancetonullify;


    public List<AudioClip> MonsterWalkSound;
    public List<AudioClip> PlayerWalkSound;

    [Serializable]
    public class SoundEffectClass
    {
        public int objectID;
        public AudioClip GrabSFX;
        public AudioClip CrashSFX;
    }

    public List<SoundEffectClass> GrabSFXList;

    public int stepDelay;
    public float delaybetweenstepsmultiplier;

    public int MonsterStepDelay;

    public float PlayerSFXvolMult;

    public AudioSource MusicChill;
    public AudioSource MusicChase;
    public AudioSource MusicChillIntro;
    public AudioSource MusicChaseIntro;

    public float musicvolume;

    private void Awake()
    {
        instance = this;
    }


    public void ChangeVolume()
    {
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(DataScript.instance.Options.MusicVol) * 20f);
        AudioMixer.SetFloat("SoundVolume", Mathf.Log10(DataScript.instance.Options.SFXVol) * 20f);
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(DataScript.instance.Options.Mastervol) * 20f);
    }
    private void Update()
    {
        if (MovementController.instance != null)
        {
            if (MovementController.instance.GetComponent<Rigidbody>().linearVelocity.magnitude > 0.1f)
            {
                if (stepDelay == 0)
                {

                    int randomSFX = UnityEngine.Random.Range(0, PlayerWalkSound.Count);
                    stepDelay = (int)(PlayerWalkSound[randomSFX].length * 60f);
                    if (MovementController.instance.iscrouching)
                    {
                        stepDelay = (int)((float)stepDelay * 1.2f);
                    }
                    PlaySFX(PlayerWalkSound[randomSFX], 0.05f, MovementController.instance.transform, PlayerSFXvolMult);
                }
                else
                {
                    stepDelay--;
                }
            }
            else
            {
                stepDelay = 0;
            }
        }
        if (EnemyController.instance != null)
        {
            if (EnemyController.instance.GetComponent<NavMeshAgent>().velocity.magnitude > 0.1f)
            {
                if (MonsterStepDelay == 0)
                {

                    int randomSFX = UnityEngine.Random.Range(0, MonsterWalkSound.Count);
                    MonsterStepDelay = (int)(MonsterWalkSound[randomSFX].length * 60f);

                    PlaySFX(MonsterWalkSound[randomSFX], 0.05f, EnemyController.instance.transform);
                }
                else
                {
                    MonsterStepDelay--;
                }
            }
            else
            {
                MonsterStepDelay = 0;
            }
        }


        if (EnemyController.instance != null)
        {
            if (!MusicChill.isPlaying)
            {
                Debug.Log("Here");
                PlayMusicWithIntro(MusicChill, MusicChillIntro);
                PlayMusicWithIntro(MusicChase, MusicChaseIntro);
                MusicChase.volume = 0;
                MusicChaseIntro.volume = 0;
                MusicChill.volume = musicvolume;
                MusicChillIntro.volume = musicvolume;
            }

            bool chasing = EnemyController.instance.chasing;
            if (chasing)
            {
                if (MusicChase.volume < musicvolume)
                {
                    MusicChase.volume += Time.deltaTime;
                    MusicChaseIntro.volume += Time.deltaTime;
                }
                if (MusicChill.volume > 0)
                {
                    MusicChill.volume -= Time.deltaTime;
                    MusicChillIntro.volume -= Time.deltaTime;
                }
            }
            else
            {
                if (MusicChill.volume < musicvolume)
                {
                    MusicChill.volume += Time.deltaTime;
                    MusicChillIntro.volume += Time.deltaTime;
                }
                if (MusicChase.volume > 0)
                {
                    MusicChase.volume -= Time.deltaTime;
                    MusicChaseIntro.volume -= Time.deltaTime;
                }
            }
        }
        else
        {
            MusicChill.Stop();
            MusicChillIntro.Stop();
            MusicChase.Stop();
            MusicChaseIntro.Stop();
        }


    }

    private void PlayMusicWithIntro(AudioSource Main, AudioSource intro)
    {
        Main.volume = musicvolume;
        if (intro.clip == null)
        {
            Main.PlayScheduled(AudioSettings.dspTime);
        }
        else
        {
            intro.volume = musicvolume;

            intro.PlayScheduled(AudioSettings.dspTime);

            double introduration = (double)intro.clip.samples / intro.clip.frequency;


            Main.PlayScheduled(AudioSettings.dspTime + introduration);
        }


    }

    public void PlaySFX(AudioClip clip, float pitchrandomness, Transform Emiter, float volume = -1)
    {
        StartCoroutine(PlaySFXCoroutine(clip, pitchrandomness, Emiter, volume));
    }

    private IEnumerator PlaySFXCoroutine(AudioClip clip, float pitchrandomness, Transform Emiter, float newvolume)
    {

        float vol = newvolume;
        if (vol < 0)
        {
            vol = SFXVol;
        }

        GameObject newAudioSource = new GameObject();
        newAudioSource.name = clip.name;
        newAudioSource.transform.parent = SFXHolder;
        newAudioSource.transform.position = Emiter.position;
        IndividualSoundScript ISS = newAudioSource.AddComponent<IndividualSoundScript>();
        ISS.basevolume = vol;
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

        AS.volume = vol * volume;
        AS.pitch = 1f + UnityEngine.Random.Range(-pitchrandomness, pitchrandomness);
        AS.Play();
        yield return new WaitForSeconds(AS.clip.length);
        Destroy(newAudioSource);
    }




}
