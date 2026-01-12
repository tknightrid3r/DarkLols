using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonfirePromptUI : MonoBehaviour
{
    public static BonfirePromptUI Instance;

    public TextMeshProUGUI promptText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Hide prompt by default
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    public void ShowPrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
}
