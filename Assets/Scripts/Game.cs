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
    public Text highScoreText;
    public GameObject gameOverUI;
    public LeaderboardUI leaderboardUI;
    public Text startPromptText; // New: "Press any arrow key to start" text
    public GameObject startPromptPanel; // New: Optional panel for start prompt

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
        UpdateHighScoreUI();

        // Apply difficulty settings
        if (GameSettings.Instance != null)
        {
            spawnObstacles = GameSettings.Instance.ShouldSpawnObstacles();
            spawnPortals = GameSettings.Instance.ShouldSpawnPortals();
            
            // Apply wall traversal setting to snake
            if (snake != null)
            {
                snake.moveThroughWalls = GameSettings.Instance.CanMoveThroughWalls();
            }
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Show start prompt
        ShowStartPrompt(true);

        // Initialize portals based on difficulty
        if (basePortal != null)
        {
            if (spawnPortals)
            {
                basePortal.gameObject.SetActive(true);
                basePortal.gridArea = gridArea;
                basePortal.RandomizePosition();
                activePortals.Add(basePortal);
                
                SpawnPortal();
                portalsInitialized = true;
            }
            else
            {
                // Hide base portal when portals shouldn't spawn
                basePortal.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Shows or hides the start prompt
    /// </summary>
    private void ShowStartPrompt(bool show)
    {
        if (startPromptText != null)
        {
            startPromptText.gameObject.SetActive(show);
            if (show)
            {
                startPromptText.text = "Press any arrow key to start";
                // Animate the prompt
                StartCoroutine(AnimateStartPrompt());
            }
        }
        
        if (startPromptPanel != null)
        {
            startPromptPanel.SetActive(show);
        }
    }

    private IEnumerator AnimateStartPrompt()
    {
        if (startPromptText == null) yield break;
        
        Color originalColor = startPromptText.color;
        float time = 0f;
        
        while (startPromptText != null && startPromptText.gameObject.activeSelf)
        {
            time += Time.deltaTime;
            float alpha = 0.5f + Mathf.Sin(time * 3f) * 0.5f;
            startPromptText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    private void Update()
    {
        // Check if the game has started (player moved the snake)
        if (snake != null && snake.HasStarted())
        {
            ShowStartPrompt(false);
            
            // Start background music when game begins
            if (AudioManager.Instance != null && !AudioManager.Instance.IsMusicPlaying())
            {
                AudioManager.Instance.PlayBackgroundMusic();
            }
        }
        
        if (isGameOver && !isRestarting)
        {
            // Enter para restart
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                isRestarting = true;
                RestartGame();
                return;
            }

            // L para ver leaderboard
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (leaderboardUI != null)
                {
                    leaderboardUI.Show();
                }
            }

            return;
        }

        // Pause with ESC to view leaderboard during game
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.L))
        {
            if (leaderboardUI != null)
            {
                leaderboardUI.Show();
            }
        }

        // Only spawn obstacles and portals after game has started
        if (snake != null && !snake.HasStarted())
        {
            return;
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
        
        // Play game over sound and stop music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBackgroundMusic();
            AudioManager.Instance.PlayGameOverSound();
        }
        
        // Get player name
        string playerName = "Player";
        if (GameSettings.Instance != null)
        {
            playerName = GameSettings.Instance.playerName;
        }
        
        // Always save score to leaderboard (if score > 0)
        if (LeaderboardManager.Instance != null && score > 0)
        {
            bool isHighScore = LeaderboardManager.Instance.IsHighScore(score);
            LeaderboardManager.Instance.AddScore(score, playerName);
            
            if (isHighScore)
            {
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
        
        // Show start prompt again
        ShowStartPrompt(true);
    }

    public void OnSnakeReset()
    {
        if (isGameOver || isRestarting) return;
        
        // Only trigger game over if the game has actually started
        if (snake != null && !snake.HasStarted()) return;
        
        GameOver();
    }
}