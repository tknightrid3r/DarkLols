using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject bonfirePrefab;
    public int bonfireCount = 4;
    public static List<RoomBehaviour> AllRooms = new List<RoomBehaviour>();

    [Header("Enemy Spawning")]
    public GameObject enemyPrefab;
    public int enemyCount = 6;
    public int minDistanceFromStart = 3;

    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }

    }

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Vector2 offset;

    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator();
    }

    void GenerateDungeon()
    {

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }


               
                    var newRoom = Instantiate(
                    rooms[randomRoom].room,
                    new Vector3(i * offset.x, 0, -j * offset.y),
                    Quaternion.identity,
                    transform
                    ).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;

                    AllRooms.Add(newRoom);

                }
            }
        }
        GenerateBonfires();
        GenerateEnemies();
    }
    void GenerateBonfires()
    {
        List<int> validCells = new List<int>();

        // Collect all visited cells
        for (int i = 0; i < board.Count; i++)
        {
            if (board[i].visited)
            {
                validCells.Add(i);
            }
        }

        // Shuffle list
        for (int i = 0; i < validCells.Count; i++)
        {
            int rnd = Random.Range(i, validCells.Count);
            int temp = validCells[i];
            validCells[i] = validCells[rnd];
            validCells[rnd] = temp;
        }

        // Spawn bonfires
        for (int i = 0; i < bonfireCount && i < validCells.Count; i++)
        {
            int cellIndex = validCells[i];

            int x = cellIndex % size.x;
            int y = cellIndex / size.x;

            Vector3 spawnPos = new Vector3(
                x * offset.x,
                0,
                -y * offset.y
            );

            Instantiate(bonfirePrefab, spawnPos, Quaternion.identity, transform);
            BonfireManager.Instance.SetTotalBonfires(bonfireCount);
        }
    }
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
    void GenerateEnemies()
    {
        List<int> validCells = new List<int>();

        for (int i = 0; i < board.Count; i++)
            if (board[i].visited)
                validCells.Add(i);

        Shuffle(validCells);

        for (int i = 0; i < enemyCount && i < validCells.Count; i++)
        {
            int cellIndex = validCells[i];
            int x = cellIndex % size.x;
            int y = cellIndex / size.x;

            Vector3 spawnPos = new Vector3(
                x * offset.x,
                0,
                -y * offset.y
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);

            AssignRoomPatrol(enemy.transform.position, enemy);
        }
    }

    void AssignRoomPatrol(Vector3 enemyPos, GameObject enemy)
    {
        float closest = float.MaxValue;
        RoomBehaviour bestRoom = null;

        foreach (RoomBehaviour room in AllRooms)
        {
            float dist = Vector3.Distance(enemyPos, room.transform.position);
            if (dist < closest)
            {
                closest = dist;
                bestRoom = room;
            }
        }

        if (bestRoom == null) return;

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai == null) return;

        ai.patrolPoints = bestRoom.patrolPoints.ToArray();
    }
    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeon();
        NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }
}
