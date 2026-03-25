
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;


[RequireComponent(typeof(Rigidbody))]
public class FlipperController : MonoBehaviour
{
    [Header("左右设置")]
    public bool isLeftFlipper = true;
    public bool reverseAxis = false;

    [Header("把 pinnlend 拖进来")]
    public Transform tableRoot;

    [Header("角度")]
    public float restAngle = 0f;
    public float activeAngle = -60f;

    [Header("速度（度/秒）")]
    public float flipUpSpeed = 400f;
    public float flipDownSpeed = 200f;

    private Rigidbody rb;
    private Quaternion initialWorldRot;
    private Vector3 hingeAxis;
    private float currentAngle = 0f;
    private bool isFlipping = false;
    private bool hasReachedTop = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        initialWorldRot = transform.rotation;
        Vector3 axis = tableRoot != null ? tableRoot.up : Vector3.up;
        hingeAxis = reverseAxis ? -axis : axis;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isFlipping = true;
            hasReachedTop = false;
        }
    }

    void FixedUpdate()
    {
        float target;
        float speed;

        if (isFlipping && !hasReachedTop)
        {
            target = activeAngle;
            speed = flipUpSpeed;
            currentAngle = Mathf.MoveTowards(currentAngle, target, speed * Time.fixedDeltaTime);
            if (Mathf.Approximately(currentAngle, activeAngle))
                hasReachedTop = true;
        }
        else
        {
            target = restAngle;
            speed = flipDownSpeed;
            currentAngle = Mathf.MoveTowards(currentAngle, target, speed * Time.fixedDeltaTime);
            if (Mathf.Approximately(currentAngle, restAngle))
                isFlipping = false;
        }

        float sign = isLeftFlipper ? 1f : -1f;
        Quaternion delta = Quaternion.AngleAxis(sign * currentAngle, hingeAxis);
        rb.MoveRotation(delta * initialWorldRot);
    }
}