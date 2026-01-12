using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BonfireManager : MonoBehaviour
{
    public static BonfireManager Instance;

    [Header("Bonfire Count")]
    public int totalBonfires = 0;   // Set dynamically from DungeonGenerator
    private int collectedBonfires = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetTotalBonfires(int total)
    {
        totalBonfires = total;
        collectedBonfires = 0;
        BonfireUI.Instance.UpdateUI(collectedBonfires, totalBonfires);
    }

    public void CollectBonfire()
    {
        collectedBonfires++;
        BonfireUI.Instance.UpdateUI(collectedBonfires, totalBonfires);

        if (collectedBonfires >= totalBonfires)
        {
            Debug.Log("All bonfires collected! You win!");
            PlayerCam cam = FindObjectOfType<PlayerCam>();
            if (cam != null) cam.UnlockCursor();
            SceneManager.LoadScene(4);
        }
    }
}