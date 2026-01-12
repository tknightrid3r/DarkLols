using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonfireUI : MonoBehaviour
{
    public static BonfireUI Instance;

    public TextMeshProUGUI bonfireText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateUI(int collected, int total)
    {
        if (bonfireText != null)
        {
            bonfireText.text = $"Bonfires: {collected} / {total}";
        }
    }
}
