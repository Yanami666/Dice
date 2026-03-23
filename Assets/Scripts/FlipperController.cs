
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FlipperController : MonoBehaviour
{
    [Header("左右设置")]
    public bool isLeftFlipper = true;

    [Header("角度（相对初始位置）")]
    public float restAngle = 0f;      // 静止下垂角度
    public float activeAngle = 35f;   // 按Space时翻起角度

    [Header("速度（度/秒）")]
    public float flipUpSpeed = 1200f;   // 翻起速度要快，这样打球才有力
    public float flipDownSpeed = 400f;

    private Rigidbody rb;
    private float currentAngle;
    private Quaternion initialLocalRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;    // Kinematic = 不受重力，但能传递速度给球
        rb.useGravity = false;

        currentAngle = restAngle;
        initialLocalRotation = transform.localRotation;
    }

    void FixedUpdate()
    {
        bool pressing = Keyboard.current.spaceKey.isPressed;
        float targetAngle = pressing ? activeAngle : restAngle;
        float speed = pressing ? flipUpSpeed : flipDownSpeed;

        // MoveTowards比Lerp更线性，打球力道更稳定
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.fixedDeltaTime);

        // 左右镜像：左flipper正转，右flipper反转
        float angle = isLeftFlipper ? currentAngle : -currentAngle;

        // 必须用MoveRotation！这样物理引擎才知道flipper的速度，碰球时才会给力
        Quaternion localTarget = initialLocalRotation * Quaternion.Euler(0f, angle, 0f);
        Quaternion worldTarget = (transform.parent != null ? transform.parent.rotation : Quaternion.identity) * localTarget;
        rb.MoveRotation(worldTarget);
    }
}