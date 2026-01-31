
using System.Collections.Generic;
using TMPro;
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

    [Header("Clues")]
    public List<int> heldClues;

    public float timeforpickingupclues;
    private int currentpickingup;
    public UnityEngine.UI.Image ClueImage;
    public TextMeshProUGUI HeldItemsText;

    private int delayforpickingup;

    private InputAction clickInput;

    private void Start()
    {
        SoundManager = SoundManager.instance;
        interractInput = InputSystem.actions.FindAction("Interact");
        clickInput = InputSystem.actions.FindAction("PickUpItem");
        HeldItemsText.text = "Clues picked up : 0/5";
        delayforpickingup = (int)(timeforpickingupclues / Time.deltaTime);
    }


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

        if (clickInput.ReadValue<float>() == 1f)
        {
            GameObject closestobj = FindCloset();
            if (closestobj != null && closestobj.GetComponent<ThrowObjectScript>() != null && closestobj.GetComponent<ThrowObjectScript>().isclue)
            {
                currentpickingup++;

                if (currentpickingup >= delayforpickingup)
                {
                    heldClues.Add(closestobj.GetComponent<ThrowObjectScript>().clueID);
                    if (heldClues.Count < 5)
                    {
                        HeldItemsText.text = "Clues picked up : " + heldClues.Count + "/5";
                    }
                    else
                    {
                        HeldItemsText.text = "All clues found, reach the exit !";
                    }


                    GetComponent<UnderLineCloseObjects>().RemoveObjectFromList(closestobj);
                    Destroy(closestobj);
                }
            }
            else
            {
                delayforpickingup = (int)(timeforpickingupclues / Time.deltaTime);
                currentpickingup = 0;
            }
        }
        else
        {
            delayforpickingup = (int)(timeforpickingupclues / Time.deltaTime);
            currentpickingup = 0;
            if (interractInput.ReadValue<float>() != 0f)
            {
                Interact();
            }
        }

        if (heldClues.Count >= 5)
        {
            EnemyController.instance.chasing = true;
        }

        ClueImage.fillAmount = (float)currentpickingup / (float)delayforpickingup;

        if (InteractCDCounter > 0)
        {
            InteractCDCounter--;
        }



    }


    public GameObject FindCloset()
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
        return closestobj;
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
            GameObject closestobj = FindCloset();
            if (closestobj != null && !closestobj.GetComponent<ThrowObjectScript>().isclue)
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
        SoundManager.PlaySFX(obj.GetComponent<ThrowObjectScript>().GrabSFX, 0.05f, transform.position);

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
            SoundManager.PlaySFX(throwObjectSFX, 0.05f, transform.position);
            Rigidbody RB = CurrentObjectPickedUp.GetComponentInChildren<Rigidbody>();
            RB.isKinematic = false;
            CurrentObjectPickedUp.transform.parent = null;
            RB.AddForce(cam.forward * throwforce * CurrentObjectPickedUp.GetComponent<ThrowObjectScript>().sizemultiplier, ForceMode.Impulse);
            CurrentObjectPickedUp = null;
        }
    }

}
