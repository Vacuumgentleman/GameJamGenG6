using TMPro;
using UnityEngine;

public class BurnCounter : MonoBehaviour
{
    public static BurnCounter Instance;

    [Header("UI")]
    public TextMeshProUGUI counterText;

    [Header("Config")]
    public int totalNeeded = 3;
    private int burnedCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddBurned()
    {
        burnedCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (counterText != null)
        {
            counterText.text = $"Quemados: {burnedCount} / {totalNeeded}";
        }

        if (burnedCount >= totalNeeded)
        {
            counterText.text = $"¡COMPLETADO! {burnedCount}/{totalNeeded}";
            counterText.color = Color.green;
        }
    }

    public int GetCount() => burnedCount;
}