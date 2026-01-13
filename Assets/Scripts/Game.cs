using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [Header("Game Entities")]
    public Collider2D gridArea;
    public Snake snake;
    private int score;

    [Header("Food Settings")]
    public Food food;
    public int pointsPerFood = 10;

    [Header("Obstacle Settings")]
    public GameObject obstaclePrefab;
    public bool spawnObstacles = true;
    public float obstacleSpawnInterval = 5f;

    private readonly List<Obstacle> activeObstacles = new();
    private float obstacleTimer;

    [Header("Portal Settings")]
    public Portal basePortal; // Reference to the base portal in the scene
    public GameObject portalPrefab;
    public bool spawnPortals = true;
    public float portalSpawnInterval = 10f;

    private readonly List<Portal> activePortals = new();
    private float portalTimer;
    private bool portalsInitialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        obstacleTimer = obstacleSpawnInterval;
        portalTimer = portalSpawnInterval;

        // Initialize base portal if it exists
        if (spawnPortals && basePortal != null)
        {
            basePortal.gridArea = gridArea;
            basePortal.RandomizePosition();
            activePortals.Add(basePortal);
            
            // Spawn only one clone portal to pair with the base
            SpawnPortal();
            portalsInitialized = true;
        }
    }

    private void Update()
    {
        if (spawnObstacles)
        {
            obstacleTimer -= Time.deltaTime;
            if (obstacleTimer <= 0f)
            {
                SpawnObstacle();
                obstacleTimer = obstacleSpawnInterval;
            }
        }

        if (spawnPortals)
        {
            // Only handle timer-based movement if portals are initialized
            if (activePortals.Count == 2 && portalsInitialized)
            {
                portalTimer -= Time.deltaTime;
                if (portalTimer <= 0f)
                {
                    MovePortals();
                    portalTimer = portalSpawnInterval;
                }
            }
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefab == null || gridArea == null)
        {
            return;
        }

        GameObject newObstacle = Instantiate(obstaclePrefab);
        Obstacle obstacleComponent = newObstacle.GetComponent<Obstacle>();

        if (obstacleComponent != null)
        {
            obstacleComponent.gridArea = gridArea;
            obstacleComponent.RandomizePosition();
            activeObstacles.Add(obstacleComponent);
        }
    }

    public void ClearObstacles()
    {
        foreach (Obstacle obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle.gameObject);
            }
        }
        activeObstacles.Clear();
    }

    private void SpawnPortal()
    {
        if (portalPrefab == null || gridArea == null || activePortals.Count >= 2)
        {
            return;
        }

        GameObject newPortal = Instantiate(portalPrefab);
        Portal portalComponent = newPortal.GetComponent<Portal>();

        if (portalComponent != null)
        {
            portalComponent.gridArea = gridArea;
            portalComponent.RandomizePosition();
            activePortals.Add(portalComponent);

            // If we now have 2 portals, connect them
            if (activePortals.Count == 2)
            {
                activePortals[0].connectPortal(activePortals[1]);
                activePortals[1].connectPortal(activePortals[0]);
            }
        }
    }

    private void MovePortals()
    {
        // Reposition both portals to new random locations
        foreach (Portal portal in activePortals)
        {
            if (portal != null)
            {
                portal.RandomizePosition();
            }
        }
    }

    public void ClearPortals()
    {
        foreach (Portal portal in activePortals)
        {
            if (portal != null)
            {
                Destroy(portal.gameObject);
            }
        }
        activePortals.Clear();
    }

    public bool PositionOccupied(int x, int y)
    {
        // Check if snake occupies this position
        if (snake != null && snake.Occupies(x, y))
        {
            return true;
        }

        // Check if food occupies this position
        if (food != null)
        {
            Vector2 foodPos = food.transform.position;
            if (Mathf.RoundToInt(foodPos.x) == x && Mathf.RoundToInt(foodPos.y) == y)
            {
                return true;
            }
        }

        // Check if any obstacle occupies this position
        foreach (Obstacle obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                Vector2 obstaclePos = obstacle.transform.position;
                if (Mathf.RoundToInt(obstaclePos.x) == x && Mathf.RoundToInt(obstaclePos.y) == y)
                {
                    return true;
                }
            }
        }

        // Check if any portal occupies this position
        foreach (Portal portal in activePortals)
        {
            if (portal != null)
            {
                Vector2 portalPos = portal.transform.position;
                if (Mathf.RoundToInt(portalPos.x) == x && Mathf.RoundToInt(portalPos.y) == y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void OnSnakeReset()
    {
        ClearObstacles();
        MovePortals(); // Move portals instead of clearing them
        obstacleTimer = obstacleSpawnInterval;
        portalTimer = portalSpawnInterval;
    }

}
