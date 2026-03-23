using UnityEngine;
using UnityEngine.InputSystem;
using Vector3 = UnityEngine.Vector3;

public class BallController : MonoBehaviour
{
    public Vector3 launchForce = new Vector3(0f, 2f, -30f);
    public float deathY = -5f;
    public float maxSpeed = 25f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private bool launched = false;
    private BallTrail ballTrail;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballTrail = GetComponent<BallTrail>();
        startPosition = transform.position;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        PhysicsMaterial mat = new PhysicsMaterial();
        mat.dynamicFriction = 0f;
        mat.staticFriction = 0f;
        mat.bounciness = 0.3f;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.bounceCombine = PhysicsMaterialCombine.Average;
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

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void LaunchBall()
    {
        launched = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.AddForce(launchForce, ForceMode.Impulse);
    }

    void ResetBall()
    {
        launched = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        transform.position = startPosition;
        if (ballTrail != null) ballTrail.ClearTrail();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
            ResetBall();
    }
}
