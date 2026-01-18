using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [Header("Game Entities")]
    public Collider2D gridArea;
    public Snake snake;
    private int score;

    [Header("UI")]
    public Text scoreText;
    public Text highScoreText; // Novo: mostrar recorde durante o jogo
    public GameObject gameOverUI;
    public LeaderboardUI leaderboardUI; // Novo

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
    public Portal basePortal;
    public GameObject portalPrefab;
    public bool spawnPortals = true;
    public float portalSpawnInterval = 10f;

    private readonly List<Portal> activePortals = new();
    private float portalTimer;
    private bool portalsInitialized = false;

    private bool isGameOver = false;
    private bool isRestarting = false;

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
        score = 0;
        UpdateScoreUI();
        UpdateHighScoreUI(); // Novo

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Initialize base portal if it exists
        if (spawnPortals && basePortal != null)
        {
            basePortal.gridArea = gridArea;
            basePortal.RandomizePosition();
            activePortals.Add(basePortal);
            
            SpawnPortal();
            portalsInitialized = true;
        }
    }

    private void Update()
    {
        if (isGameOver && !isRestarting)
        {
            // Enter para restart
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                isRestarting = true;
                RestartGame();
                return;
            }

            // Novo: L para ver leaderboard
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (leaderboardUI != null)
                {
                    leaderboardUI.Show();
                }
            }

            return;
        }

        // Novo: Pausa com ESC para ver leaderboard durante o jogo
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.L))
        {
            if (leaderboardUI != null)
            {
                leaderboardUI.Show();
            }
        }

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

            if (activePortals.Count == 2)
            {
                activePortals[0].connectPortal(activePortals[1]);
                activePortals[1].connectPortal(activePortals[0]);
            }
        }
    }

    private void MovePortals()
    {
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
        if (snake != null && snake.Occupies(x, y))
        {
            return true;
        }

        if (food != null)
        {
            Vector2 foodPos = food.transform.position;
            if (Mathf.RoundToInt(foodPos.x) == x && Mathf.RoundToInt(foodPos.y) == y)
            {
                return true;
            }
        }

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

    public void AddScore()
    {
        score += pointsPerFood;
        UpdateScoreUI();
        
        // Novo: Atualiza high score em tempo real
        if (LeaderboardManager.Instance != null)
        {
            int topScore = LeaderboardManager.Instance.GetTopScore();
            if (score > topScore)
            {
                UpdateHighScoreUI();
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // Novo método
    private void UpdateHighScoreUI()
    {
        if (highScoreText != null && LeaderboardManager.Instance != null)
        {
            int topScore = LeaderboardManager.Instance.GetTopScore();
            highScoreText.text = "High Score: " + topScore;
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        
        // Novo: Registra score no leaderboard com nome do jogador
        if (LeaderboardManager.Instance != null)
        {
            bool isHighScore = LeaderboardManager.Instance.IsHighScore(score);
            
            if (isHighScore)
            {
                // Pega o nome do jogador das configurações
                string playerName = "Player";
                if (GameSettings.Instance != null)
                {
                    playerName = GameSettings.Instance.playerName;
                }
                
                LeaderboardManager.Instance.AddScore(score, playerName);
                int rank = LeaderboardManager.Instance.GetScoreRank(score);
                
                // Mostra indicador de novo recorde
                if (leaderboardUI != null)
                {
                    leaderboardUI.ShowNewRecordIndicator(rank);
                }
            }
        }
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        yield return null;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        // Novo: Esconde indicador de recorde
        if (leaderboardUI != null)
        {
            leaderboardUI.HideNewRecordIndicator();
        }

        isGameOver = false;
        isRestarting = false;

        Time.timeScale = 1f;

        score = 0;
        UpdateScoreUI();
        UpdateHighScoreUI(); // Novo

        ClearObstacles();
        MovePortals();

        if (snake != null)
            snake.ResetState();

        if (food != null)
            food.RandomizePosition();

        obstacleTimer = obstacleSpawnInterval;
        portalTimer = portalSpawnInterval;
    }

    public void OnSnakeReset()
    {
        if (isGameOver || isRestarting) return;
        GameOver();
    }
}