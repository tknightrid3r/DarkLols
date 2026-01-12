using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    [Header("Walls / Doors (Order: Up, Right, Down, Left)")]
    public GameObject[] walls;
    public List<Transform> patrolPoints = new List<Transform>();

    void Awake()
    {
        patrolPoints.Clear();

        foreach (Transform child in transform)
        {
            if (child.CompareTag("PatrolPoint"))
                patrolPoints.Add(child);
        }
    }
    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < walls.Length && i < status.Length; i++)
        {
            // If status[i] == true, opening exists → disable wall
            walls[i].SetActive(!status[i]);
        }
    }
    public Transform GetRandomPatrolPoint()
    {
        if (patrolPoints.Count == 0)
            return null;

        return patrolPoints[Random.Range(0, patrolPoints.Count)];
    }
}
