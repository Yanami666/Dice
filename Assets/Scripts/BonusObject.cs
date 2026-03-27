
using UnityEngine;

public class BonusObject : MonoBehaviour
{
    [Header("碰到后加的分数")]
    public float bonusPoints = 500f;
    public ScoreManager scoreManager;

    void Start()
    {
        // 如果没有手动赋值就自动找
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        if (scoreManager != null)
            scoreManager.AddScore(bonusPoints);
        else
            Debug.LogWarning("BonusObject: ScoreManager 是空的！");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        if (scoreManager != null)
            scoreManager.AddScore(bonusPoints);
    }
}