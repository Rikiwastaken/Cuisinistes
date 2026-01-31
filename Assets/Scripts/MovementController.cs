using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MovementController : MonoBehaviour
{

    public static MovementController instance;


    [Header("Movement")]
    public Transform CameraTransform;
    private Rigidbody rb;

    private InputAction MoveAction;

    public float speed;

    public Transform handtransform;

    [Header("FlashLight")]

    private InputAction FlashLightToggle;

    private float previousflashlightstate;

    public AudioClip flashlight;


    [Header("Arm Sway")]

    public float swaymax;

    private bool goingUp;

    public float changeperframe;

    private float baseYHand;

    [Header("Light Cone Variables")]
    public Light Light;

    public float maxangle;
    private float lightrange;
    public LayerMask enemylayer;
    public LayerMask ObsctacleLayer;

    [Header("Enemy Close Visuals")]
    public Volume EnemyCloseVolume;

    public float minimumweight;
    public float distancewhenstarts;
    public float distanceformax;

    private void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MoveAction = InputSystem.actions.FindAction("Move");
        FlashLightToggle = InputSystem.actions.FindAction("FlashLight");
        baseYHand = handtransform.localPosition.y;
    }


    void Update()
    {

        Vector2 MoveValue = MoveAction.ReadValue<Vector2>();

        if (MoveValue.magnitude != 0)
        {
            Vector3 movement = new Vector3(MoveValue.x * speed, 0.0f, MoveValue.y * speed);

            movement = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0) * movement;



            movement.y = rb.linearVelocity.y;


            rb.linearVelocity = movement;




            //armsway

            if (goingUp)
            {
                handtransform.localPosition += new Vector3(0f, changeperframe * Time.deltaTime, 0f);
                if (handtransform.localPosition.y > baseYHand + swaymax)
                {
                    goingUp = !goingUp;
                }
            }
            else
            {
                handtransform.localPosition -= new Vector3(0f, changeperframe * Time.deltaTime, 0f);
                if (handtransform.localPosition.y < baseYHand - swaymax)
                {
                    goingUp = !goingUp;
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.5f);
        }


        // FlashLight


        if (FlashLightToggle.ReadValue<float>() != 0f && previousflashlightstate == 0f)
        {
            Light.enabled = !Light.enabled;
            SoundManager.instance.PlaySFX(flashlight, 0.1f);
        }


        previousflashlightstate = FlashLightToggle.ReadValue<float>();

        //enemyclose visuals

        float distancetoenemy = Vector3.Distance(EnemyController.instance.transform.position, transform.position);

        if (distancetoenemy <= distancewhenstarts)
        {
            EnemyCloseVolume.weight = minimumweight + ((distancewhenstarts - distancetoenemy) / (distancewhenstarts - distanceformax)) * (1f - minimumweight);
        }
        else
        {
            EnemyCloseVolume.weight = 0f;
        }

        //Manage Enemy seeing Light
        if (Light.enabled)
        {
            if (checkifenemyislit())
            {
                EnemyController.instance.seeingplayer = true;
            }
        }


    }
    private bool checkifenemyislit()
    {
        lightrange = Light.range * 0.9f;
        Vector3 enemypos = EnemyController.instance.transform.position;

        if (Vector3.Distance(enemypos, Light.transform.position) > lightrange)
        {
            return false;
        }
        Vector3 toenemy = enemypos - Light.transform.position;

        //Check angle for alignment
        Vector3 directionToEnemy = toenemy.normalized;
        float angle = Vector3.Angle(Light.transform.forward, directionToEnemy);
        if (angle > maxangle / 2f)
        {
            return false;
        }
        //Raycast
        if (Physics.Raycast(Light.transform.position, directionToEnemy, out RaycastHit hit, lightrange, ObsctacleLayer | enemylayer))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                return true;
            }
        }
        return false;

    }
}
