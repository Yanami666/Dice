
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
        Debug.Log($"ScoreObject 初始化：{gameObject.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter：{other.gameObject.name} Tag：{other.tag}");
        HandleHit(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"OnCollisionEnter：{collision.gameObject.name} Tag：{collision.gameObject.tag}");
        HandleHit(collision.gameObject);
    }

    void HandleHit(GameObject other)
    {
        if (!other.CompareTag("Ball")) return;
        if (Time.time - lastHitTime < cooldown) return;

        lastHitTime = Time.time;

        if (scoreManager != null)
            scoreManager.AddScore(points);

        if (energyManager != null)
            energyManager.RegisterHit(gameObject);
        else
            Debug.LogWarning($"EnergyManager 是空的！{gameObject.name}");

        if (hitSound != null)
        {
            audioSource.clip = hitSound;
            audioSource.Play();
        }
    }
}