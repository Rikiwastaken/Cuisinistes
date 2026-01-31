using UnityEngine;
using static SoundManager;

public class ThrowObjectScript : MonoBehaviour
{

    public float sizemultiplier;

    public bool isclue;

    public int SFXID;

    public AudioClip GrabSFX;
    public AudioClip CrasHSFX;

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

}
