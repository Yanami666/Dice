using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class EnergyManager : MonoBehaviour
{
    [Header("触发设置")]
    public int hitsRequired = 5;
    public GameObject[] triggerObjects;

    [Header("Fever Time 设置")]
    public float feverDuration = 20f;
    public Material feverMaterial;
    public Renderer[] feverTargets;

    [Header("效果1：复制球")]
    public GameObject ballPrefab;
    public Vector3 duplicateLaunchForce = new Vector3(0f, 0.1f, -6f);

    [Header("UI")]
    public Slider energySlider;
    public TextMeshProUGUI energyText;
    public GameObject[] abilityIcons;

    private int currentHits = 0;
    private bool isReady = false;
    private bool isFever = false;
    private float feverTimer = 0f;
    private GameObject duplicateBall;
    private Material[] originalMaterials;

    void Start()
    {
        if (feverTargets != null)
        {
            originalMaterials = new Material[feverTargets.Length];
            for (int i = 0; i < feverTargets.Length; i++)
            {
                if (feverTargets[i] != null)
                    originalMaterials[i] = feverTargets[i].material;
            }
        }

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

        if (isFever)
        {
            feverTimer -= Time.deltaTime;
            if (feverTimer <= 0f)
                EndFever();
        }
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

    void OnEnergyFull()
    {
        ApplyFeverMaterial();
        Debug.Log("能量满了！材质已变化，按1触发效果");

        if (abilityIcons != null)
            foreach (var icon in abilityIcons)
                if (icon != null) icon.SetActive(true);
    }

    void TriggerAbility(int index)
    {
        if (!isReady) return;

        if (index == 0)
        {
            isFever = true;
            feverTimer = feverDuration;
            SpawnDuplicateBall();
        }
        // if (index == 1) { 效果2 }
        // if (index == 2) { 效果3 }

        currentHits = 0;
        isReady = false;
        UpdateUI();
    }

    void ApplyFeverMaterial()
    {
        if (feverMaterial == null || feverTargets == null) return;

        foreach (var r in feverTargets)
        {
            if (r != null)
                r.material = feverMaterial;
        }
    }

    void EndFever()
    {
        isFever = false;
        RestoreMaterials();
        Debug.Log("Fever Time 结束，材质恢复");
    }

    void RestoreMaterials()
    {
        if (feverTargets == null || originalMaterials == null) return;

        for (int i = 0; i < feverTargets.Length; i++)
        {
            if (feverTargets[i] != null && originalMaterials[i] != null)
                feverTargets[i].material = originalMaterials[i];
        }
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
            bc.launchForce = duplicateLaunchForce;
            bc.LaunchImmediately();
        }

        Destroy(duplicateBall, feverDuration);
        Debug.Log($"复制球生成，{feverDuration}秒后消失");
    }

    public void ResetEnergy()
    {
        currentHits = 0;
        isReady = false;
        isFever = false;
        feverTimer = 0f;

        // 球死了无论什么状态都恢复材质
        RestoreMaterials();

        // 销毁复制球
        if (duplicateBall != null)
        {
            Destroy(duplicateBall);
            duplicateBall = null;
        }

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