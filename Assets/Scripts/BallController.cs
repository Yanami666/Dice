using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("发射设置")]
    public Vector3 launchForce = new Vector3(0f, 0.1f, -6f);

    [Header("速度限制")]
    public float maxSpeed = 80f;

    [Header("被Flipper打到时的速度倍增")]
    public float flipperSpeedMultiplier = 1.8f;

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

        PhysicsMaterial mat = new PhysicsMaterial();
        mat.dynamicFriction = 0f;
        mat.staticFriction = 0f;
        mat.bounciness = 0f;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.bounceCombine = PhysicsMaterialCombine.Minimum;
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

        Vector3 normal = tableNormal.normalized;
        Vector3 velocity = rb.linearVelocity;
        float normalComponent = Vector3.Dot(velocity, normal);
        if (normalComponent > 0)
            rb.linearVelocity = velocity - normalComponent * normal;

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!launched) return;

        // 被flipper打到时加速
        if (collision.gameObject.CompareTag("Flipper"))
        {
            Vector3 vel = rb.linearVelocity;
            float speed = vel.magnitude;

            // 确保有最小速度再乘倍数
            float newSpeed = Mathf.Max(speed, 5f) * flipperSpeedMultiplier;
            newSpeed = Mathf.Min(newSpeed, maxSpeed);
            rb.linearVelocity = vel.normalized * newSpeed;
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