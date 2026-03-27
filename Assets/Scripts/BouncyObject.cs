
using UnityEngine;

public class BouncyObject : MonoBehaviour
{
    [Header("弹力大小")]
    public float bounceForce = 30f;

    [Header("冷却时间（防止连续触发）")]
    public float cooldown = 0.3f;

    private float lastBounceTime = -999f;

    void Start()
    {
        PhysicsMaterial mat = new PhysicsMaterial("Bouncy");
        mat.bounciness = 0f;        // 改成0，弹力由代码控制
        mat.dynamicFriction = 0f;
        mat.staticFriction = 0f;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.bounceCombine = PhysicsMaterialCombine.Minimum;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.material = mat;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;
        if (Time.time - lastBounceTime < cooldown) return;

        lastBounceTime = Time.time;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 normal = collision.contacts[0].normal;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(normal * bounceForce, ForceMode.Impulse);
    }
}