using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BallTrail : MonoBehaviour
{
    [Header("拖尾基本设置")]
    public float trailTime = 0.3f;
    public float startWidth = 0.3f;
    public float endWidth = 0.0f;

    [Header("颜色")]
    public Color startColor = new Color(1f, 0.8f, 0.2f, 1f);
    public Color endColor = new Color(1f, 0.2f, 0.1f, 0f);

    private TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();

        trail.time = trailTime;
        trail.startWidth = startWidth;
        trail.endWidth = endWidth;
        trail.minVertexDistance = 0.05f;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(startColor.a, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = gradient;

        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
    }

    public void ClearTrail()
    {
        if (trail != null)
            trail.Clear();
    }
}
