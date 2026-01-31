using UnityEngine;
using static SoundManager;

public class ThrowObjectScript : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (CDBeforeFirstSound == 100000)
        {
            SoundManager.instance.PlaySFX(CrasHSFX, 0.15f);
            CDBeforeFirstSound = 0;
        }

    }


    public float sizemultiplier;

    public bool isclue;

    public int SFXID;

    public AudioClip GrabSFX;
    public AudioClip CrasHSFX;

    private int CDBeforeFirstSound;

    private void Start()
    {
        foreach (SoundEffectClass SFXClass in SoundManager.instance.GrabSFXList)
        {
            if (SFXClass.objectID == SFXID)
            {
                GrabSFX = SFXClass.GrabSFX;
                CrasHSFX = SFXClass.CrashSFX;
            }
        }
    }

    private void Update()
    {
        if (CDBeforeFirstSound < 0.25f / Time.deltaTime)
        {
            CDBeforeFirstSound++;
        }
        else
        {
            CDBeforeFirstSound = 100000;
        }
    }

}
