using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    public bool chasing;

    public NavMeshAgent agent;


    public float distancefordetection;

    public float viewradius;

    public float eyeHeight;

    public LayerMask obstaclemask;
    public LayerMask playermask;

    public bool seeingplayer;

    [Header("Debug")]

    public bool debug;

    public int stunframes;

    public float basestun;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("CanBeTaken") && collision.transform.GetComponent<ThrowObjectScript>() && collision.transform.GetComponentInChildren<Rigidbody>().linearVelocity.magnitude >= 0.2f)
        {
            stunframes = (int)(basestun / (Time.deltaTime * collision.transform.GetComponent<ThrowObjectScript>().sizemultiplier));
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (chasing && stunframes == 0)
        {
            agent.isStopped = false;
            agent.SetDestination(MovementController.instance.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }
        if (stunframes > 0)
        {
            stunframes--;
        }

        seeingplayer = isplayerdetected();
    }


    private bool isplayerdetected()
    {
        Transform playertransform = MovementController.instance.transform;
        Vector3 toplayer = playertransform.position - transform.position;

        // Check distance
        if (toplayer.magnitude > distancefordetection)
        {
            return false;
        }

        //Check angle for alignment
        Vector3 directionToPlayer = toplayer.normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewradius / 2f)
        {
            return false;
        }

        //Raycast
        if (Physics.Raycast(transform.position + Vector3.up * eyeHeight, directionToPlayer, out RaycastHit hit, viewradius, obstaclemask | playermask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (!debug) return;

        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distancefordetection);

        // Eye position
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;

        // Draw forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(eyePos, eyePos + transform.forward * distancefordetection);

        // Draw vision cone edges
        float halfFOV = viewradius * 0.5f;

        Vector3 leftBoundary = Quaternion.Euler(0, -halfFOV, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, halfFOV, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(eyePos, eyePos + leftBoundary * distancefordetection);
        Gizmos.DrawLine(eyePos, eyePos + rightBoundary * distancefordetection);

        // Draw ray to player (if exists)
        if (MovementController.instance != null)
        {
            Transform player = MovementController.instance.transform;
            Vector3 toPlayer = (player.position - eyePos).normalized;
            float dist = Vector3.Distance(eyePos, player.position);

            if (dist <= distancefordetection)
            {
                bool hitPlayer = false;

                if (Physics.Raycast(eyePos, toPlayer, out RaycastHit hit, distancefordetection, obstaclemask | playermask))
                {
                    hitPlayer = hit.transform.gameObject.layer == LayerMask.NameToLayer("Player");
                }

                Gizmos.color = hitPlayer ? Color.green : Color.red;
                Gizmos.DrawLine(eyePos, eyePos + toPlayer * Mathf.Min(dist, distancefordetection));
            }
        }
    }
}
