
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FlipperController : MonoBehaviour
{
    [Header("左右设置")]
    public bool isLeftFlipper = true;

    [Header("如果方向反了就勾这个")]
    public bool reverseAxis = false;

    [Header("把 pinnlend 拖进来")]
    public Transform tableRoot;

    [Header("角度")]
    public float restAngle = 0f;
    public float activeAngle = 30f;

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
            // 往上翻，必须到顶
            target = activeAngle;
            speed = flipUpSpeed;

            currentAngle = Mathf.MoveTowards(currentAngle, target, speed * Time.fixedDeltaTime);

            // 到达顶部了
            if (Mathf.Approximately(currentAngle, activeAngle))
                hasReachedTop = true;
        }
        else
        {
            // 到顶了或者没在翻：往下回
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