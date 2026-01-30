using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpObjects : MonoBehaviour
{

    public GameObject CurrentObjectPickedUp;

    public Transform ObjectHolder;

    public float throwforce;

    public Transform cam;

    private InputAction interractInput;

    private void Start()
    {
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

    }

    private void Interact()
    {
        Debug.Log("interact");
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
        CurrentObjectPickedUp = obj;
        CurrentObjectPickedUp.transform.parent = ObjectHolder;
        Rigidbody RB = CurrentObjectPickedUp.GetComponentInChildren<Rigidbody>();
        RB.isKinematic = true;
        CurrentObjectPickedUp.transform.localPosition = Vector3.zero;
    }

    public void throwObject()
    {
        if (CurrentObjectPickedUp != null)
        {

            Rigidbody RB = CurrentObjectPickedUp.GetComponentInChildren<Rigidbody>();
            RB.isKinematic = false;
            CurrentObjectPickedUp.transform.parent = null;
            RB.AddForce(cam.forward * throwforce, ForceMode.Impulse);
            CurrentObjectPickedUp = null;
        }
    }

}
