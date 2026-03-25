using UnityEngine;
using TMPro;
using System;


public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("时间得分")]
    public float pointsPerSecond = 10f;

    [Header("碰撞得分目标（拖进来）")]
    public ScoreTarget[] scoreTargets;

    private float score = 0f;
    private bool gameActive = false;

    void Start()
    {
        UpdateUI(); // ← 加这行，初始化显示 Score: 0
    }

    void Update()
    {
        if (!gameActive) return;
        score += pointsPerSecond * Time.deltaTime;
        UpdateUI();
    }

    public void StartScoring()
    {
        gameActive = true;
    }

    public void StopScoring()
    {
        gameActive = false;
    }

    public void AddScore(float points)
    {
        score += points;
        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0f;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        else
            Debug.LogWarning("Score Text 没有赋值！");
    }
}

[System.Serializable]
public class ScoreTarget
{
    public GameObject target;
    public float points = 100f;
    public AudioClip hitSound;
}