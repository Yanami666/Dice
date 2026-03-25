using UnityEngine;

public class ScoreObject : MonoBehaviour
{
    [Header("ScoreManager 拖进来")]
    public ScoreManager scoreManager;

    [Header("EnergyManager 拖进来")]
    public EnergyManager energyManager;

    [Header("得分和音效")]
    public float points = 100f;
    public AudioClip hitSound;

    [Header("冷却时间")]
    public float cooldown = 0.5f;

    private float lastHitTime = -999f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;
        if (Time.time - lastHitTime < cooldown) return;

        lastHitTime = Time.time;

        if (scoreManager != null)
            scoreManager.AddScore(points);

        // 传入自身，让EnergyManager判断是否是有效触发物体
        if (energyManager != null)
            energyManager.RegisterHit(gameObject);

        if (hitSound != null)
        {
            audioSource.clip = hitSound;
            audioSource.Play();
        }
    }
}