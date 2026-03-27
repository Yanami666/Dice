
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BallLauncher : MonoBehaviour
{
    [Header("触发这个效果的物体（拖进来）")]
    public GameObject triggerObject;

    [Header("终点（球飞向这里）")]
    public Transform endPoint;

    [Header("弧线高度")]
    public float arcHeight = 3f;

    [Header("飞行时间（秒）")]
    public float flightDuration = 0.8f;

    [Header("到达终点后的初始速度")]
    public float arrivalSpeed = 5f;

    private Rigidbody rb;
    private bool isAnimating = false;
    private float timer = 0f;
    private Vector3 arcStart;
    private Vector3 arcEnd;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isAnimating) return;
        if (triggerObject == null || endPoint == null) return;
        if (collision.gameObject != triggerObject) return;

        StartArcAnimation();
    }

    void StartArcAnimation()
    {
        isAnimating = true;
        timer = 0f;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 从当前位置飞向终点
        arcStart = transform.position;
        arcEnd = endPoint.position;
    }

    void Update()
    {
        if (!isAnimating) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flightDuration);

        Vector3 mid = (arcStart + arcEnd) / 2f + Vector3.up * arcHeight;

        Vector3 pos = Mathf.Pow(1 - t, 2) * arcStart
                    + 2 * (1 - t) * t * mid
                    + Mathf.Pow(t, 2) * arcEnd;

        transform.position = pos;

        if (t >= 1f)
        {
            isAnimating = false;
            transform.position = arcEnd;

            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            Vector3 direction = (arcEnd - arcStart).normalized;
            rb.AddForce(direction * arrivalSpeed, ForceMode.Impulse);
        }
    }
}