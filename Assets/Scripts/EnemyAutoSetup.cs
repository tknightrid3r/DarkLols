using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAutoSetup : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            EnemyAI ai = GetComponent<EnemyAI>();
            if (ai != null)
                ai.player = player.transform;
        }
    }
}
