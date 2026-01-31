using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpObjects : MonoBehaviour
{

    public GameObject CurrentObjectPickedUp;

    public Transform ObjectHolder;

    public float throwforce;

    public Transform cam;

    private InputAction interractInput;

    public float interactCD;

    private int InteractCDCounter;

    private SoundManager SoundManager;

    public AudioClip throwObjectSFX;

    private void Start()
    {
        SoundManager = SoundManager.instance;
        interractInput = InputSystem.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentObjectPickedUp != null)
        {
            if (CurrentObjectPickedUp.transform.parent != ObjectHolder)
            {
                CurrentObjectPickedUp.transform.parent = ObjectHolder;
                Rigidbody RB = CurrentObjectPickedUp.GetComponentInChildren<Rigidbody>();
                RB.isKinematic = true;
            }
        }

        if (interractInput.ReadValue<float>() != 0f)
        {
            Interact();
        }

        if (InteractCDCounter > 0)
        {
            InteractCDCounter--;
        }

    }

    private void Interact()
    {
        if (InteractCDCounter == 0)
        {
            InteractCDCounter = (int)(interactCD / Time.deltaTime);
        }
        else
        {
            return;
        }
        if (CurrentObjectPickedUp == null)
        {
            float mindist = GetComponent<UnderLineCloseObjects>().minimaldistance;
            GameObject closestobj = null;
            foreach (GameObject obj in GetComponent<UnderLineCloseObjects>().objectspickable)
            {
                float distance = Vector3.Distance(obj.transform.position, transform.position);
                if (distance <= mindist)
                {
                    closestobj = obj;
                    mindist = distance;
                }
            }
            if (closestobj != null)
            {
                PickUpObject(closestobj);
            }
        }
        else
        {
            throwObject();
        }




    }
    public void PickUpObject(GameObject obj)
    {
        SoundManager.PlaySFX(obj.GetComponent<ThrowObjectScript>().GrabSFX, 0.05f);

        CurrentObjectPickedUp = obj;
        CurrentObjectPickedUp.transform.parent = ObjectHolder;
        Rigidbody RB = CurrentObjectPickedUp.GetComponentInChildren<Rigidbody>();
        RB.isKinematic = true;
        CurrentObjectPickedUp.transform.localPosition = Vector3.zero;
        CurrentObjectPickedUp.transform.localRotation = Quaternion.identity;
    }

    public void throwObject()
    {
        if (CurrentObjectPickedUp != null)
        {
            SoundManager.PlaySFX(throwObjectSFX, 0.05f);
            Rigidbody RB = CurrentObjectPickedUp.GetComponentInChildren<Rigidbody>();
            RB.isKinematic = false;
            CurrentObjectPickedUp.transform.parent = null;
            RB.AddForce(cam.forward * throwforce * CurrentObjectPickedUp.GetComponent<ThrowObjectScript>().sizemultiplier, ForceMode.Impulse);
            CurrentObjectPickedUp = null;
        }
    }

}
