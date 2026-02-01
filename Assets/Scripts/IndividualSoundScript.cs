using UnityEngine;

public class IndividualSoundScript : MonoBehaviour
{

    public Transform Emiter;

    public Transform PlayerTransform;

    public float maxdistancetonullify;

    public float basevolume;

    private Vector3 previousposEmiterPos;

    // Update is called once per frame
    void Update()
    {
        float volume = 0;

        if (Emiter != null)
        {
            float distance = Vector3.Distance(PlayerTransform.position, Emiter.position);


            if (distance < maxdistancetonullify)
            {
                volume = (maxdistancetonullify - distance) / maxdistancetonullify;
            }
        }
        else
        {
            float distance = Vector3.Distance(PlayerTransform.position, previousposEmiterPos);


            if (distance < maxdistancetonullify)
            {
                volume = (maxdistancetonullify - distance) / maxdistancetonullify;
            }
        }



        GetComponent<AudioSource>().volume = basevolume * volume;

        if (Emiter != null)
        {
            previousposEmiterPos = Emiter.position;
        }

    }
}
