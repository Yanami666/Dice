using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;


public class BallController : MonoBehaviour
{
    [Header("发射设置")]
    public Vector3 launchForce = new Vector3(0f, 0.1f, -15f);

    [Header("速度限制")]
    public float maxSpeed = 150f;

    [Header("被Flipper打到时的速度倍增")]
    public float flipperSpeedMultiplier = 2.5f;

    [Header("弹力（碰到任何东西的反弹力度）")]
    public float bounceMultiplier = 1.2f;

    [Header("重置Y轴阈值")]
    public float deathY = -5f;

    [Header("台面法线方向")]
    public Vector3 tableNormal = new Vector3(0f, 0.96f, 0.28f);

    [Header("ScoreManager")]
    public ScoreManager scoreManager;

    [Header("EnergyManager")]
    public EnergyManager energyManager;

    private Rigidbody rb;
    private Vector3 startPosition;
    private bool launched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // 高弹力材质
        PhysicsMaterial mat = new PhysicsMaterial();
        mat.dynamicFriction = 0f;
        mat.staticFriction = 0f;
        mat.bounciness = 0.8f;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.bounceCombine = PhysicsMaterialCombine.Maximum;
        GetComponent<SphereCollider>().material = mat;
    }

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (!launched && Keyboard.current.spaceKey.wasPressedThisFrame)
            LaunchBall();

        if (launched && transform.position.y < deathY)
            ResetBall();
    }

    void FixedUpdate()
    {
        if (!launched) return;

        rb.angularVelocity = Vector3.zero;

        // 去掉防弹起的逻辑，让球自然弹跳
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!launched) return;

        if (collision.gameObject.CompareTag("Flipper"))
        {
            // Flipper打到时速度倍增
            Vector3 vel = rb.linearVelocity;
            float speed = vel.magnitude;
            float newSpeed = Mathf.Max(speed, 10f) * flipperSpeedMultiplier;
            newSpeed = Mathf.Min(newSpeed, maxSpeed);
            rb.linearVelocity = vel.normalized * newSpeed;
        }
        else
        {
            // 碰到其他物体时保持弹力
            Vector3 vel = rb.linearVelocity;
            if (vel.magnitude < 5f)
            {
                Vector3 normal = collision.contacts[0].normal;
                rb.linearVelocity = Vector3.Reflect(vel, normal) * bounceMultiplier;
            }
        }
    }

    public void LaunchBall()
    {
        launched = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce(launchForce, ForceMode.Impulse);
        scoreManager?.StartScoring();
    }

    public void LaunchImmediately()
    {
        launched = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce(launchForce, ForceMode.Impulse);
        scoreManager?.StartScoring();
    }

    public void ResetBall()
    {
        if (gameObject.name.Contains("(Clone)"))
        {
            Destroy(gameObject);
            return;
        }

        launched = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        transform.position = startPosition;
        scoreManager?.StopScoring();
        scoreManager?.ResetScore();
        energyManager?.ResetEnergy();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
            ResetBall();
    }
}