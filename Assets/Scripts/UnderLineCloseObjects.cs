using System.Linq;
using UnityEngine;

public class UnderLineCloseObjects : MonoBehaviour
{


    public float minimaldistance;

    public string pickableTag;

    public GameObject[] objectspickable;

    public Material outlinematerialForPickable;
    public Material outlinematerialForClue;


    public LayerMask layermask;

    public Transform cam;

    public float maxdistanceforraycast;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectspickable = GameObject.FindGameObjectsWithTag(pickableTag);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject obj in objectspickable)
        {
            RaycastHit hit;
            Ray ray = new Ray(cam.position, cam.forward);

            if (Physics.Raycast(ray, out hit, maxdistanceforraycast, layermask))
            {
                if (hit.collider.transform.tag == pickableTag)
                {
                    AddMaterial(obj);
                    continue;
                }
            }

            if (Vector3.Distance(obj.transform.position, transform.position) <= minimaldistance)
            {
                AddMaterial(obj);
            }
            else
            {
                RemoveMaterial(obj);
            }
        }
    }


    private void AddMaterial(GameObject obj)
    {
        MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            var mats = mesh.sharedMaterials.ToList();

            if (obj.GetComponent<ThrowObjectScript>() != null && obj.GetComponent<ThrowObjectScript>().isclue)
            {
                if (!mats.Contains(outlinematerialForClue))
                {
                    mats.Add(outlinematerialForClue);
                    mesh.sharedMaterials = mats.ToArray();
                }
            }
            else
            {
                if (!mats.Contains(outlinematerialForPickable))
                {
                    mats.Add(outlinematerialForPickable);
                    mesh.sharedMaterials = mats.ToArray();
                }
            }

        }
    }

    private void RemoveMaterial(GameObject obj)
    {
        MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            var mats = mesh.sharedMaterials.ToList();

            if (mats.Contains(outlinematerialForClue))
            {
                mats.Remove(outlinematerialForClue);
                mesh.sharedMaterials = mats.ToArray();
            }

            mats = mesh.sharedMaterials.ToList();

            if (mats.Contains(outlinematerialForPickable))
            {
                mats.Remove(outlinematerialForPickable);
                mesh.sharedMaterials = mats.ToArray();
            }
        }
    }
}
