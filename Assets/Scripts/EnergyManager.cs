using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class EnergyManager : MonoBehaviour
{
    [Header("触发设置")]
    public int hitsRequired = 5;
    public GameObject[] triggerObjects;

    [Header("UI")]
    public Slider energySlider;
    public TextMeshProUGUI energyText;
    public GameObject[] abilityIcons;

    private int currentHits = 0;
    private bool isReady = false;

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

        Debug.Log($"触发效果 {index + 1}");

        // 效果逻辑以后在这里加
        // if (index == 0) { 效果1 }
        // if (index == 1) { 效果2 }
        // if (index == 2) { 效果3 }

        currentHits = 0;
        isReady = false;
        UpdateUI();
    }

    void OnEnergyFull()
    {
        Debug.Log("能量满了！按1/2/3触发效果");

        if (abilityIcons != null)
            foreach (var icon in abilityIcons)
                if (icon != null) icon.SetActive(true);
    }

    public void ResetEnergy()
    {
        currentHits = 0;
        isReady = false;
        UpdateUI();
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