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

    [Header("弹力")]
    public float bounceMultiplier = 1.2f;

    [Header("重置Y轴阈值")]
    public float deathY = -5f;

    [Header("台面法线方向")]
    public Vector3 tableNormal = new Vector3(0f, 0.96f, 0.28f);

    [Header("死亡后激活的物体")]
    public GameObject deathObject;
    public float lockDuration = 5f;

    [Header("龙卷风设置")]
    public float tornadoDuration = 10f;
    public float tornadoFloatHeight = 2f;
    public float tornadoOrbitRadius = 3f;
    public float tornadoSpinSpeed = 180f;
    public float tornadoFlySpeed = 10f;
    public float tornadoPointsPerSecond = 20f;
    public Transform tornadoTarget;

    [Header("ScoreManager")]
    public ScoreManager scoreManager;

    [Header("EnergyManager")]
    public EnergyManager energyManager;

    private Rigidbody rb;
    private Vector3 startPosition;
    private bool launched = false;
    public static bool isLocked = false;

    private bool isTornado = false;
    private bool tornadoFlying = false;
    private float tornadoTimer = 0f;
    private Vector3 tornadoCenter;
    private Vector3 tornadoFlyTarget;

    void Awake()
    {
        isLocked = false;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        PhysicsMaterial mat = new PhysicsMaterial();
        mat.dynamicFriction = 0f;
        mat.staticFriction = 0f;
        mat.bounciness = 0.8f;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.bounceCombine = PhysicsMaterialCombine.Maximum;
        GetComponent<SphereCollider>().material = mat;

        if (deathObject != null)
            deathObject.SetActive(false);
    }

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isLocked) return;
        if (isTornado) return;

        if (!launched && Keyboard.current.spaceKey.wasPressedThisFrame)
            LaunchBall();

        if (launched && transform.position.y < deathY)
            OnBallDead();
    }

    void FixedUpdate()
    {
        if (isTornado)
        {
            UpdateTornado();
            return;
        }

        if (!launched) return;

        rb.angularVelocity = Vector3.zero;

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    public void StartTornado()
    {
        if (!launched) return;

        isTornado = true;
        tornadoFlying = true;
        tornadoTimer = tornadoDuration;

        if (tornadoTarget != null)
            tornadoCenter = tornadoTarget.position + new Vector3(0f, tornadoFloatHeight, 0f);
        else
            tornadoCenter = transform.position + new Vector3(0f, tornadoFloatHeight, 0f);

        tornadoFlyTarget = tornadoCenter + new Vector3(tornadoOrbitRadius, 0f, 0f);

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void UpdateTornado()
    {
        if (tornadoFlying)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                tornadoFlyTarget,
                tornadoFlySpeed * Time.fixedDeltaTime
            );

            if (Vector3.Distance(transform.position, tornadoFlyTarget) < 0.1f)
            {
                transform.position = tornadoFlyTarget;
                tornadoFlying = false;
            }
            return;
        }

        tornadoTimer -= Time.fixedDeltaTime;
        transform.RotateAround(
            tornadoCenter,
            Vector3.up,
            tornadoSpinSpeed * Time.fixedDeltaTime
        );

        scoreManager?.AddScore(tornadoPointsPerSecond * Time.fixedDeltaTime);

        if (tornadoTimer <= 0f)
            EndTornado();
    }

    void EndTornado()
    {
        isTornado = false;
        tornadoFlying = false;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!launched) return;
        if (isTornado) return;

        // 弹性物体自己处理，不干预
        if (collision.gameObject.GetComponent<BouncyObject>() != null) return;

        if (collision.gameObject.CompareTag("Flipper"))
        {
            Vector3 vel = rb.linearVelocity;
            float speed = vel.magnitude;
            float newSpeed = Mathf.Max(speed, 10f) * flipperSpeedMultiplier;
            newSpeed = Mathf.Min(newSpeed, maxSpeed);
            rb.linearVelocity = vel.normalized * newSpeed;
        }
        else
        {
            Vector3 vel = rb.linearVelocity;
            if (vel.magnitude < 5f)
            {
                Vector3 normal = collision.contacts[0].normal;
                rb.linearVelocity = Vector3.Reflect(vel, normal) * bounceMultiplier;
            }
        }
    }

    void OnBallDead()
    {
        if (isLocked) return;

        if (isTornado)
        {
            isTornado = false;
            tornadoFlying = false;
        }

        launched = false;
        isLocked = true;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        transform.position = startPosition;

        energyManager?.ResetEnergy();

        if (deathObject != null)
            deathObject.SetActive(true);

        scoreManager?.StartDeathSequence(lockDuration);
        Invoke(nameof(Unlock), lockDuration);
    }

    void Unlock()
    {
        isLocked = false;

        if (deathObject != null)
            deathObject.SetActive(false);

        scoreManager?.StopScoring();
        scoreManager?.ResetScore();
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

        OnBallDead();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
            OnBallDead();
    }
}