using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;



    public NavMeshAgent agent;

    [Header("Detection")]
    public bool chasing;
    public float distancefordetection;

    public float viewradius;

    public float eyeHeight;

    public LayerMask obstaclemask;
    public LayerMask playermask;
    public LayerMask Pickablemask;

    public bool seeingplayer;






    [Header("Movement")]
    public Vector3 Destination;

    public float speedwhenchasing;

    public float speedwhenchill;

    public float staytime;

    public int staystimecounter = -1;

    public int notmoveing;

    public int stunframes;

    public float basestun;

    [Header("ChaseVariables")]

    public float timebeforegiveupchase;
    public int timebeforegiveupchaseCounter;

    public float chasedetectiondistance;

    public float chaseviewradius;

    public float delaybetweenchecks;
    private int delaybetweenchecksCounter;

    [Header("Debug")]

    public bool debug;
    public float pushforce;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("CanBeTaken") && collision.transform.GetComponent<ThrowObjectScript>())
        {
            if (collision.transform.GetComponentInChildren<Rigidbody>().linearVelocity.magnitude >= 0.2f)
            {
                stunframes = (int)(basestun / (Time.deltaTime * collision.transform.GetComponent<ThrowObjectScript>().sizemultiplier));
                MovementController.instance.GetComponent<UnderLineCloseObjects>().RemoveObjectFromList(collision.transform.gameObject);
                Destroy(collision.transform.gameObject);
            }
            else
            {
                Vector3 direction = collision.transform.position - transform.position + new Vector3(0, 1, 0);
                direction.Normalize();

                collision.transform.GetComponent<Rigidbody>().AddForce(direction * pushforce, ForceMode.Impulse);
            }

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
        if (chasing)
        {
            agent.speed = speedwhenchasing;
            agent.isStopped = false;

            staystimecounter = -1;

            timebeforegiveupchaseCounter++;
            if (seeingplayer)
            {
                Destination = MovementController.instance.transform.position;
                timebeforegiveupchaseCounter = 0;
            }
            if (timebeforegiveupchaseCounter > timebeforegiveupchase / Time.deltaTime)
            {
                chasing = false;
            }
        }
        else
        {
            agent.speed = speedwhenchill / 2f + (speedwhenchill / 2f) * ((float)MovementController.instance.transform.GetComponent<PickUpObjects>().heldClues.Count / 4f);
            agent.isStopped = false;
            if ((Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(Destination.x, Destination.z)) < 1f || Destination == null || Destination == Vector3.zero) && staystimecounter == -1)
            {
                staystimecounter = (int)(staytime / Time.deltaTime);


            }
            if (staystimecounter == 0 || notmoveing > 2f * staytime / Time.deltaTime)
            {
                Destination = GetRandomLocation();
                staystimecounter = -1;
            }
            else if (staystimecounter > 0)

            {
                staystimecounter--;
            }
        }
        agent.SetDestination(Destination);



        if (stunframes > 0)
        {
            agent.isStopped = true;
            stunframes--;
        }


        if (delaybetweenchecksCounter == 0)
        {
            seeingplayer = isplayerdetected();
            delaybetweenchecksCounter = (int)(delaybetweenchecks / Time.deltaTime);
        }
        else
        {
            delaybetweenchecksCounter--;
        }



        if (seeingplayer && !chasing)
        {
            chasing = true;
        }

        if (agent.velocity.magnitude < 0.1f)
        {
            notmoveing++;
        }
        else
        {
            notmoveing = 0;
        }

        GetComponentInChildren<Animator>().SetFloat("speed", agent.velocity.magnitude);
    }


    private bool isplayerdetected()
    {
        Transform playertransform = MovementController.instance.transform;
        Vector3 toplayer = playertransform.position - transform.position;

        float detectiondistancetouse = 0f;
        if (chasing)
        {
            detectiondistancetouse = chasedetectiondistance;
        }
        else
        {
            detectiondistancetouse = distancefordetection;
        }



        float viewangletouse = 0f;
        if (chasing)
        {
            viewangletouse = chaseviewradius;
        }
        else
        {
            viewangletouse = viewradius;
        }

        ManageLightShape(detectiondistancetouse, viewangletouse);

        if (MovementController.instance.iscrouching)
        {
            detectiondistancetouse /= 2f;
        }

        // Check distance
        if (toplayer.magnitude > detectiondistancetouse)
        {
            return false;
        }

        // Check distance
        if (toplayer.magnitude < detectiondistancetouse / 3f)
        {
            return true;
        }

        //Check angle for alignment
        Vector3 directionToPlayer = toplayer.normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewangletouse / 2f)
        {
            return false;
        }

        //Raycast
        if (Physics.Raycast(transform.position + Vector3.up * eyeHeight, directionToPlayer, out RaycastHit hit, viewangletouse, obstaclemask | playermask | Pickablemask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void ManageLightShape(float range, float angle)
    {
        Light light = GetComponent<Light>();

        light.range = range;
        light.innerSpotAngle = angle / 2;
        light.spotAngle = angle;
    }

    private Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;

        // pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        int secondVertexSelected = UnityEngine.Random.Range(0, maxIndices);

        // spawn on verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];

        // eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x || (int)firstVertexPosition.z == (int)secondVertexPosition.z)
        {
            point = GetRandomLocation(); // re-roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // select a random point on it
            point = Vector3.Lerp(firstVertexPosition, secondVertexPosition, UnityEngine.Random.Range(0.05f, 0.95f));
        }



        return point;
    }

    void OnDrawGizmosSelected()
    {
        if (!debug) return;

        float detectiondistancetouse = 0f;
        if (chasing)
        {
            detectiondistancetouse = chasedetectiondistance;
        }
        else
        {
            detectiondistancetouse = distancefordetection;
        }

        if (MovementController.instance.iscrouching)
        {
            detectiondistancetouse /= 2f;
        }

        float viewangletouse = 0f;
        if (chasing)
        {
            viewangletouse = chaseviewradius;
        }
        else
        {
            viewangletouse = viewradius;
        }


        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectiondistancetouse);

        // Eye position
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;

        // Draw forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(eyePos, eyePos + transform.forward * detectiondistancetouse);

        // Draw vision cone edges
        float halfFOV = viewangletouse * 0.5f;

        Vector3 leftBoundary = Quaternion.Euler(0, -halfFOV, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, halfFOV, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(eyePos, eyePos + leftBoundary * detectiondistancetouse);
        Gizmos.DrawLine(eyePos, eyePos + rightBoundary * detectiondistancetouse);

        // Draw ray to player (if exists)
        if (MovementController.instance != null)
        {
            Transform player = MovementController.instance.transform;
            Vector3 toPlayer = (player.position - eyePos).normalized;
            float dist = Vector3.Distance(eyePos, player.position);

            if (dist <= distancefordetection)
            {
                bool hitPlayer = false;

                if (Physics.Raycast(eyePos, toPlayer, out RaycastHit hit, detectiondistancetouse, obstaclemask | playermask | Pickablemask))
                {
                    hitPlayer = hit.transform.gameObject.layer == LayerMask.NameToLayer("Player");
                }

                Gizmos.color = hitPlayer ? Color.green : Color.red;
                Gizmos.DrawLine(eyePos, eyePos + toPlayer * Mathf.Min(dist, detectiondistancetouse));
            }
        }
    }
}
