using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{

    public static MovementController instance;

    public Transform CameraTransform;
    private Rigidbody rb;

    private InputAction MoveAction;

    public float speed;

    private Vector3 targetcamrotation;

    public Transform handtransform;

    public float swaymax;

    private bool goingUp;

    public float changeperframe;

    private float baseYHand;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MoveAction = InputSystem.actions.FindAction("Move");
        baseYHand = handtransform.localPosition.y;
    }

    // Update is called once per frame
    // Update is called once per frame
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


    }
}
