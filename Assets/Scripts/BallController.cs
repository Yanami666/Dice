
using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("发射设置")]
    public Vector3 launchForce = new Vector3(0f, 0.1f, -6f);

    [Header("速度限制")]
    public float maxSpeed = 50f;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
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

    void LaunchBall()
    {
        launched = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce(launchForce, ForceMode.Impulse);
        scoreManager?.StartScoring();
    }

    void ResetBall()
    {
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