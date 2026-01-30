using System.Linq;
using UnityEngine;

public class UnderLineCloseObjects : MonoBehaviour
{


    public float minimaldistance;

    public string pickableTag;

    private GameObject[] objectspickable;

    public Material outlinematerial;

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
                Debug.Log(hit.collider.name);
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

            if (!mats.Contains(outlinematerial))
            {
                mats.Add(outlinematerial);
                mesh.sharedMaterials = mats.ToArray();
            }
        }
    }

    private void RemoveMaterial(GameObject obj)
    {
        MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            var mats = mesh.sharedMaterials.ToList();

            if (mats.Contains(outlinematerial))
            {
                mats.Remove(outlinematerial);
                mesh.sharedMaterials = mats.ToArray();
            }
        }
    }
}
