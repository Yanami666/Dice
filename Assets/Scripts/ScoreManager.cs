using UnityEngine;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("时间得分")]
    public float pointsPerSecond = 10f;

    [Header("闪烁设置")]
    public float blinkInterval = 0.2f;

    private float score = 0f;
    private bool gameActive = false;
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private bool blinkVisible = true;

    public float scoreMultiplier = 1f; // 分数倍数

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (gameActive)
        {
            score += pointsPerSecond * scoreMultiplier * Time.deltaTime;
            UpdateUI();
        }

        if (isBlinking)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f)
            {
                blinkVisible = !blinkVisible;
                blinkTimer = blinkInterval;
                if (scoreText != null)
                    scoreText.enabled = blinkVisible;
            }
        }
    }

    public void StartDeathSequence(float duration)
    {
        gameActive = false;
        isBlinking = true;
        blinkTimer = blinkInterval;
        blinkVisible = true;
        Invoke(nameof(StopBlink), duration);
    }

    void StopBlink()
    {
        isBlinking = false;
        if (scoreText != null)
            scoreText.enabled = true;
    }

    public void StartScoring() => gameActive = true;
    public void StopScoring() => gameActive = false;

    public void AddScore(float points)
    {
        score += points * scoreMultiplier;
        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0f;
        scoreMultiplier = 1f;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }
}