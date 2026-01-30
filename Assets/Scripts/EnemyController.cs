using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public bool chasing;

    public NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (chasing)
        {
            agent.isStopped = false;
            agent.SetDestination(MovementController.instance.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
