using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class EnergyManager : MonoBehaviour
{
    [Header("触发设置")]
    public int hitsRequired = 5;
    public GameObject[] triggerObjects;

    [Header("效果1：复制球")]
    public GameObject ballPrefab;
    public float duplicateDuration = 20f;
    public Vector3 duplicateLaunchForce = new Vector3(0f, 0.1f, -6f); // 单独调整复制球发射力

    [Header("UI")]
    public Slider energySlider;
    public TextMeshProUGUI energyText;
    public GameObject[] abilityIcons;

    private int currentHits = 0;
    private bool isReady = false;
    private GameObject duplicateBall;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (!isReady) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TriggerAbility(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TriggerAbility(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            TriggerAbility(2);
    }

    public void RegisterHit(GameObject hitObject)
    {
        if (isReady) return;

        bool isValidTarget = false;
        foreach (var obj in triggerObjects)
        {
            if (obj == hitObject)
            {
                isValidTarget = true;
                break;
            }
        }

        if (!isValidTarget) return;

        currentHits++;
        UpdateUI();

        if (currentHits >= hitsRequired)
        {
            isReady = true;
            OnEnergyFull();
        }
    }

    void TriggerAbility(int index)
    {
        if (!isReady) return;

        if (index == 0) SpawnDuplicateBall();
        // if (index == 1) { 效果2 }
        // if (index == 2) { 效果3 }

        currentHits = 0;
        isReady = false;
        UpdateUI();
    }

    void SpawnDuplicateBall()
    {
        if (ballPrefab == null) return;

        if (duplicateBall != null)
            Destroy(duplicateBall);

        Vector3 spawnPos = ballPrefab.transform.position + new Vector3(1f, 0f, 0f);
        duplicateBall = Instantiate(ballPrefab, spawnPos, ballPrefab.transform.rotation);

        BallController bc = duplicateBall.GetComponent<BallController>();
        if (bc != null)
        {
            bc.scoreManager = ballPrefab.GetComponent<BallController>().scoreManager;
            bc.energyManager = this;

            // 用单独设置的发射力
            bc.launchForce = duplicateLaunchForce;
            bc.LaunchImmediately();
        }

        Destroy(duplicateBall, duplicateDuration);
        Debug.Log($"复制球生成，{duplicateDuration}秒后消失");
    }

    void OnEnergyFull()
    {
        Debug.Log("能量满了！按1触发复制球");
        if (abilityIcons != null)
            foreach (var icon in abilityIcons)
                if (icon != null) icon.SetActive(true);
    }

    public void ResetEnergy()
    {
        currentHits = 0;
        isReady = false;

        if (energySlider != null)
        {
            energySlider.maxValue = hitsRequired;
            energySlider.value = 0;
        }

        if (energyText != null)
            energyText.text = $"0/{hitsRequired}";

        if (abilityIcons != null)
            foreach (var icon in abilityIcons)
                if (icon != null) icon.SetActive(false);
    }

    void UpdateUI()
    {
        if (energySlider != null)
        {
            energySlider.maxValue = hitsRequired;
            energySlider.value = currentHits;
        }

        if (energyText != null)
            energyText.text = $"{currentHits}/{hitsRequired}";

        if (!isReady && abilityIcons != null)
            foreach (var icon in abilityIcons)
                if (icon != null) icon.SetActive(false);
    }

    public bool IsReady() => isReady;
}