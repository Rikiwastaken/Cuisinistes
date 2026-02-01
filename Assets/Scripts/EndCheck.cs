using UnityEngine;

public class EndCheck : MonoBehaviour
{

    public static EndCheck instance;

    public bool playerinside;

    private void Awake()
    {
        instance = this;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerinside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerinside = false;
        }
    }
}
