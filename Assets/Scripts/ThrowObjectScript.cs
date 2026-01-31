using System.Collections.Generic;
using UnityEngine;
using static SoundManager;

public class ThrowObjectScript : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (CDBeforeFirstSound == 100000)
        {
            SoundManager.instance.PlaySFX(CrasHSFX, 0.25f, transform.position);
            CDBeforeFirstSound = 0;
        }

    }

    public List<int> clueOwnerIndex; // 0 : policier, 1 : drogué, 2 : infirmière, 3 : paysan, 4 : cambrioleur

    public float sizemultiplier;

    public bool isclue;

    public int SFXID;

    public int clueID;

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
        CDBeforeFirstSound = -(int)(1 / Time.deltaTime);
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
