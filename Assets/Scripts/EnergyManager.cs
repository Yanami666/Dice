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

    [Header("Fever期间生成的弹性物体")]
    public GameObject bouncyObjectPrefab;       // 拖入弹性物体Prefab
    public Transform[] bouncySpawnPoints;       // 生成位置

    [Header("效果1：复制球")]
    public GameObject ballPrefab;
    public Vector3 duplicateLaunchForce = new Vector3(0f, 0.1f, -6f);

    [Header("效果2：Bonus物体")]
    public GameObject bonusObjectPrefab;
    public Transform[] bonusSpawnPoints;
    public ScoreManager bonusScoreManager;

    [Header("效果3：龙卷风")]
    public BallController ballController;

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
    private GameObject[] spawnedBonusObjects;
    private GameObject[] spawnedBouncyObjects;

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
        if (isReady)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                TriggerAbility(0);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                TriggerAbility(1);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                TriggerAbility(2);
        }

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

        if (abilityIcons != null)
            foreach (var icon in abilityIcons)
                if (icon != null) icon.SetActive(true);
    }

    void TriggerAbility(int index)
    {
        if (!isReady) return;

        if (isFever)
            EndFever();

        isFever = true;
        feverTimer = feverDuration;

        // 每次触发都生成弹性物体
        SpawnBouncyObjects();

        if (index == 0)
        {
            SpawnDuplicateBall();
        }
        else if (index == 1)
        {
            SpawnBonusObjects();
        }
        else if (index == 2)
        {
            if (ballController != null)
                ballController.StartTornado();
        }

        currentHits = 0;
        isReady = false;
        UpdateUI();
    }

    void SpawnBouncyObjects()
    {
        if (bouncyObjectPrefab == null || bouncySpawnPoints == null) return;

        ClearBouncyObjects();

        spawnedBouncyObjects = new GameObject[bouncySpawnPoints.Length];

        for (int i = 0; i < bouncySpawnPoints.Length; i++)
        {
            if (bouncySpawnPoints[i] == null) continue;

            GameObject obj = Instantiate(
                bouncyObjectPrefab,
                bouncySpawnPoints[i].position,
                bouncySpawnPoints[i].rotation
            );

            spawnedBouncyObjects[i] = obj;
        }
    }

    void ClearBouncyObjects()
    {
        if (spawnedBouncyObjects == null) return;

        foreach (var obj in spawnedBouncyObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedBouncyObjects = null;
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
            bc.energyManager = null;
            bc.launchForce = duplicateLaunchForce;
            bc.LaunchImmediately();
        }

        Destroy(duplicateBall, feverDuration);
    }

    void SpawnBonusObjects()
    {
        if (bonusObjectPrefab == null || bonusSpawnPoints == null) return;

        ClearBonusObjects();

        spawnedBonusObjects = new GameObject[bonusSpawnPoints.Length];

        for (int i = 0; i < bonusSpawnPoints.Length; i++)
        {
            if (bonusSpawnPoints[i] == null) continue;

            GameObject obj = Instantiate(
                bonusObjectPrefab,
                bonusSpawnPoints[i].position,
                bonusSpawnPoints[i].rotation
            );

            BonusObject bo = obj.GetComponent<BonusObject>();
            if (bo != null)
                bo.scoreManager = bonusScoreManager;

            spawnedBonusObjects[i] = obj;
            Destroy(obj, feverDuration);
        }
    }

    void ClearBonusObjects()
    {
        if (spawnedBonusObjects == null) return;

        foreach (var obj in spawnedBonusObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedBonusObjects = null;
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
        feverTimer = 0f;
        RestoreMaterials();
        ClearBonusObjects();
        ClearBouncyObjects();

        if (duplicateBall != null)
        {
            Destroy(duplicateBall);
            duplicateBall = null;
        }
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

    public void ResetEnergy()
    {
        currentHits = 0;
        isReady = false;
        isFever = false;
        feverTimer = 0f;

        RestoreMaterials();
        ClearBonusObjects();
        ClearBouncyObjects();

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