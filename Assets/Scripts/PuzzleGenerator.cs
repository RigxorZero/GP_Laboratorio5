using UnityEngine;
using System.Collections.Generic;

public class PuzzleGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject rockPrefab;
    public Transform goal;          // Puerta de salida
    public Vector2Int gridSize = new Vector2Int(6, 6);
    public int rockCount = 3;
    public GameObject Player;
    [Header("Grid Plane")]
    public Transform planeOrigin; // origen (0,0) de la 'grilla' en el plano
    public float cellSize = 1f; // tamaño de cada celda en unidades de mundo

    private List<Vector2Int> finalPositions = new List<Vector2Int>();
    private List<Vector2Int> startPositions = new List<Vector2Int>();

    void Start()
    {
        GeneratePuzzle();
    }

    void GeneratePuzzle()
    {
        // Obtener posiciones del player y la puerta en coordenadas de cuadrícula (según el plano)
        Vector2Int playerGrid = new Vector2Int(-1, -1);
        Vector2Int goalGrid = new Vector2Int(-1, -1);
        if (Player != null)
            playerGrid = WorldToGrid(Player.transform.position);
        if (goal != null)
            goalGrid = WorldToGrid(goal.position);

        // 1. Posiciones finales
        finalPositions.Clear();
        for (int i = 0; i < rockCount; i++)
        {
            Vector2Int pos;
            int attempts = 0;
            // Evitar posiciones duplicadas y evitar player/goal
            do
            {
                pos = GetRandomCell();
                attempts++;
            } while ((finalPositions.Contains(pos) || pos == playerGrid || pos == goalGrid) && attempts < 200);
            finalPositions.Add(pos);
        }

        // 2. Retroceder
        startPositions.Clear();
        foreach (var fPos in finalPositions)
        {
            Vector2Int current = fPos;
            int steps = Random.Range(1, 4);
            for (int s = 0; s < steps; s++)
            {
                Vector2Int dir = GetRandomDirection();
                Vector2Int next = current + dir;
                // Considerar límites, posición del player, la puerta y evitar solapamiento con otras rocas ya asignadas
                if (IsInsideGrid(next) && next != playerGrid && next != goalGrid && !startPositions.Contains(next))
                    current = next;
            }

            // Si por alguna razón terminamos en la posición del player o ya está ocupada, intentar buscar un vecino libre
            if (current == playerGrid || startPositions.Contains(current))
            {
                bool found = false;
                for (int attempt = 0; attempt < 10 && !found; attempt++)
                {
                    Vector2Int cand = current + GetRandomDirection();
                    if (IsInsideGrid(cand) && cand != playerGrid && cand != goalGrid && !startPositions.Contains(cand))
                    {
                        current = cand;
                        found = true;
                        break;
                    }
                }
                // Si no encontramos un vecino libre, elegir una celda aleatoria que no sea player/goal/ocupada
                if (!found)
                {
                    int attempts = 0;
                    Vector2Int cand;
                    do
                    {
                        cand = GetRandomCell();
                        attempts++;
                    } while ((cand == playerGrid || cand == goalGrid || startPositions.Contains(cand)) && attempts < 200);
                    current = cand;
                }
            }

            startPositions.Add(current);
        }

        // 3. Instanciar rocas en la posición inicial calculada
        foreach (var sPos in startPositions)
        {
            Vector3 worldPos = GridToWorld(sPos);
            Instantiate(rockPrefab, worldPos, Quaternion.identity);
        }
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        // Calcula el origen real en la esquina inferior-izquierda
        Vector3 origin = planeOrigin != null ? planeOrigin.position : Vector3.zero;
        origin.x -= (gridSize.x * 0.5f) * cellSize;
        origin.z -= (gridSize.y * 0.5f) * cellSize;

        float localX = worldPos.x - origin.x;
        float localZ = worldPos.z - origin.z;

        int gx = Mathf.FloorToInt(localX / cellSize);
        int gy = Mathf.FloorToInt(localZ / cellSize);
        return new Vector2Int(gx, gy);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector3 origin = planeOrigin != null ? planeOrigin.position : Vector3.zero;
        origin.x -= (gridSize.x * 0.5f) * cellSize;
        origin.z -= (gridSize.y * 0.5f) * cellSize;

        float x = origin.x + (gridPos.x + 0.5f) * cellSize;
        float z = origin.z + (gridPos.y + 0.5f) * cellSize;
        return new Vector3(x, 0, z);
    }


    Vector2Int GetRandomCell()
    {
        return new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
    }

    Vector2Int GetRandomDirection()
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return dirs[Random.Range(0, dirs.Length)];
    }

    bool IsInsideGrid(Vector2Int p)
    {
        return p.x >= 0 && p.x < gridSize.x && p.y >= 0 && p.y < gridSize.y;
    }
}
