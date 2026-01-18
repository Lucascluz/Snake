using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Builds and manages the in-game UI (HUD, Game Over screen, etc.) programmatically
/// Attach this to an empty GameObject in the GameScene, or let it create itself
/// </summary>
public class GameUIBuilder : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameModeMenuSceneName = "GameModeMenu";

    // UI References
    private Canvas canvas;
    private Text scoreText;
    private Text highScoreText;
    private Text startPromptText;
    private GameObject startPromptContainer;
    private GameObject gameOverPanel;
    private Text gameOverScoreText;
    private Text newRecordText;
    private GameObject leaderboardPanel;

    // Game reference
    private Game game;
    private Snake snake;
    private bool wasGameStarted = false;
    private bool wasGameOver = false;

    private void Awake()
    {
        // Find game references
        game = FindFirstObjectByType<Game>();
        snake = FindFirstObjectByType<Snake>();
    }

    private void Start()
    {
        BuildUI();
        
        // Link to Game component if exists
        if (game != null)
        {
            game.scoreText = scoreText;
            game.highScoreText = highScoreText;
            game.startPromptText = startPromptText;
            game.gameOverUI = gameOverPanel;
        }
    }

    private void Update()
    {
        // Check if game started to hide prompt (hide the entire container including gray bar)
        if (!wasGameStarted && snake != null && snake.HasStarted())
        {
            wasGameStarted = true;
            if (startPromptContainer != null)
            {
                startPromptContainer.SetActive(false);
            }
        }

        // Check if game over panel just became active - update score display
        if (gameOverPanel != null && gameOverPanel.activeSelf && !wasGameOver)
        {
            wasGameOver = true;
            UpdateGameOverScore();
        }
        else if (gameOverPanel != null && !gameOverPanel.activeSelf && wasGameOver)
        {
            wasGameOver = false;
        }

        // Handle game over input
        if (gameOverPanel != null && gameOverPanel.activeSelf)
        {
            HandleGameOverInput();
        }

        // Handle leaderboard toggle
        if (leaderboardPanel != null)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                leaderboardPanel.SetActive(!leaderboardPanel.activeSelf);
                if (leaderboardPanel.activeSelf)
                {
                    RefreshLeaderboard();
                }
            }
            
            if (leaderboardPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                leaderboardPanel.SetActive(false);
            }
        }
    }

    private void UpdateGameOverScore()
    {
        if (gameOverScoreText == null || game == null) return;
        
        // Get current score from Game using reflection or public access
        int currentScore = GetCurrentScore();
        gameOverScoreText.text = $"Score: {currentScore:N0}";
        
        // Check if it's a new record
        if (LeaderboardManager.Instance != null && newRecordText != null)
        {
            bool isNewRecord = LeaderboardManager.Instance.IsHighScore(currentScore);
            newRecordText.gameObject.SetActive(isNewRecord && currentScore > 0);
        }
    }

    private int GetCurrentScore()
    {
        // Try to get score from the scoreText UI element
        if (scoreText != null && !string.IsNullOrEmpty(scoreText.text))
        {
            string scoreString = scoreText.text.Replace("Score: ", "").Replace(",", "");
            if (int.TryParse(scoreString, out int score))
            {
                return score;
            }
        }
        return 0;
    }

    private void BuildUI()
    {
        // Create canvas
        canvas = UIBuilder.CreateCanvas("GameUICanvas");

        // === HUD ===
        BuildHUD();

        // === Start Prompt ===
        BuildStartPrompt();

        // === Game Over Panel ===
        BuildGameOverPanel();

        // === Leaderboard Panel ===
        BuildLeaderboardPanel();
    }

    private void BuildHUD()
    {
        // Score display (top left)
        GameObject scoreContainer = new GameObject("ScoreContainer");
        scoreContainer.transform.SetParent(canvas.transform, false);
        RectTransform scoreContainerRect = scoreContainer.AddComponent<RectTransform>();
        scoreContainerRect.anchorMin = new Vector2(0, 1);
        scoreContainerRect.anchorMax = new Vector2(0, 1);
        scoreContainerRect.anchoredPosition = new Vector2(20, -20);
        scoreContainerRect.sizeDelta = new Vector2(300, 100);
        scoreContainerRect.pivot = new Vector2(0, 1);

        scoreText = UIBuilder.CreateText(scoreContainer.transform, "ScoreText", "Score: 0", 32, Color.white, TextAnchor.UpperLeft);
        RectTransform scoreRect = scoreText.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(0, 0);
        scoreRect.pivot = new Vector2(0, 1);

        highScoreText = UIBuilder.CreateText(scoreContainer.transform, "HighScoreText", "High Score: 0", 22, new Color(0.7f, 0.7f, 0.75f), TextAnchor.UpperLeft);
        RectTransform highRect = highScoreText.GetComponent<RectTransform>();
        highRect.anchorMin = new Vector2(0, 1);
        highRect.anchorMax = new Vector2(0, 1);
        highRect.anchoredPosition = new Vector2(0, -40);
        highRect.pivot = new Vector2(0, 1);

        // Difficulty indicator (top right)
        string diffText = "NORMAL";
        Color diffColor = UIBuilder.NormalYellow;
        if (GameSettings.Instance != null)
        {
            diffText = GameSettings.Instance.difficulty.ToString().ToUpper();
            diffColor = GetDifficultyColor(GameSettings.Instance.difficulty);
        }

        Text diffIndicator = UIBuilder.CreateText(canvas.transform, "DifficultyIndicator", diffText, 24, diffColor, TextAnchor.UpperRight);
        RectTransform diffRect = diffIndicator.GetComponent<RectTransform>();
        diffRect.anchorMin = new Vector2(1, 1);
        diffRect.anchorMax = new Vector2(1, 1);
        diffRect.anchoredPosition = new Vector2(-20, -20);
        diffRect.pivot = new Vector2(1, 1);

        // Controls hint (bottom)
        Text controlsHint = UIBuilder.CreateText(canvas.transform, "ControlsHint", "WASD or Arrow Keys to move  •  L for Leaderboard  •  ESC for Menu", 16, new Color(0.4f, 0.4f, 0.45f));
        RectTransform controlsRect = controlsHint.GetComponent<RectTransform>();
        controlsRect.anchorMin = new Vector2(0.5f, 0);
        controlsRect.anchorMax = new Vector2(0.5f, 0);
        controlsRect.anchoredPosition = new Vector2(0, 25);
        controlsRect.sizeDelta = new Vector2(800, 30);
    }

    private void BuildStartPrompt()
    {
        startPromptContainer = new GameObject("StartPrompt");
        startPromptContainer.transform.SetParent(canvas.transform, false);
        RectTransform promptRect = startPromptContainer.AddComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.5f);
        promptRect.anchorMax = new Vector2(0.5f, 0.5f);
        promptRect.anchoredPosition = new Vector2(0, -150);
        promptRect.sizeDelta = new Vector2(500, 60);

        Image bg = startPromptContainer.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

        startPromptText = UIBuilder.CreateText(startPromptContainer.transform, "PromptText", "Press any arrow key to start", 28, UIBuilder.PrimaryGreen);
        RectTransform textRect = startPromptText.GetComponent<RectTransform>();
        UIBuilder.StretchToFill(textRect);

        // Animate the prompt
        StartCoroutine(AnimateStartPrompt());
    }

    private IEnumerator AnimateStartPrompt()
    {
        if (startPromptText == null) yield break;

        Color baseColor = startPromptText.color;
        float time = 0f;

        while (startPromptText != null && startPromptText.gameObject.activeSelf)
        {
            time += Time.deltaTime;
            float alpha = 0.5f + Mathf.Sin(time * 3f) * 0.5f;
            startPromptText.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }
    }

    private void BuildGameOverPanel()
    {
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        UIBuilder.StretchToFill(panelRect);

        // Semi-transparent background
        Image bg = gameOverPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.85f);

        // Content box
        RectTransform contentBox = UIBuilder.CreateCenteredPanel(gameOverPanel.transform, "ContentBox", 500, 450, UIBuilder.PanelBackground);

        // Game Over title
        Text gameOverTitle = UIBuilder.CreateText(contentBox, "Title", "GAME OVER", 56, UIBuilder.HardRed);
        gameOverTitle.fontStyle = FontStyle.Bold;
        RectTransform titleRect = gameOverTitle.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 160);

        // New record text (hidden by default)
        newRecordText = UIBuilder.CreateText(contentBox, "NewRecord", "🏆 NEW HIGH SCORE! 🏆", 28, new Color(1f, 0.84f, 0f));
        RectTransform recordRect = newRecordText.GetComponent<RectTransform>();
        recordRect.anchoredPosition = new Vector2(0, 95);
        newRecordText.gameObject.SetActive(false);

        // Score display
        gameOverScoreText = UIBuilder.CreateText(contentBox, "ScoreDisplay", "Score: 0", 42, Color.white);
        RectTransform scoreDispRect = gameOverScoreText.GetComponent<RectTransform>();
        scoreDispRect.anchoredPosition = new Vector2(0, 40);

        // Buttons
        VerticalLayoutGroup buttonsLayout = UIBuilder.CreateVerticalLayout(contentBox, "Buttons", 15f);
        RectTransform buttonsRect = buttonsLayout.GetComponent<RectTransform>();
        buttonsRect.anchoredPosition = new Vector2(0, -70);

        UIBuilder.CreateButton(buttonsLayout.transform, "RestartButton", "▶  PLAY AGAIN", 280, 60, UIBuilder.PrimaryGreen, OnRestartClicked);
        UIBuilder.CreateButton(buttonsLayout.transform, "MenuButton", "⌂  MAIN MENU", 280, 55, UIBuilder.ButtonNormal, OnMainMenuClicked);

        // Footer
        Text footer = UIBuilder.CreateText(contentBox, "Footer", "Press ENTER to restart  •  ESC for menu", 18, new Color(0.5f, 0.5f, 0.55f));
        RectTransform footerRect = footer.GetComponent<RectTransform>();
        footerRect.anchoredPosition = new Vector2(0, -180);

        gameOverPanel.SetActive(false);
    }

    private void BuildLeaderboardPanel()
    {
        leaderboardPanel = new GameObject("LeaderboardPanel");
        leaderboardPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = leaderboardPanel.AddComponent<RectTransform>();
        UIBuilder.StretchToFill(panelRect);

        Image bg = leaderboardPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.9f);

        RectTransform contentBox = UIBuilder.CreateCenteredPanel(leaderboardPanel.transform, "ContentBox", 520, 620, UIBuilder.PanelBackground);

        // Title
        Text title = UIBuilder.CreateText(contentBox, "Title", "🏆 LEADERBOARD", 36, UIBuilder.PrimaryGreen);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 265);

        // Column headers
        GameObject headerObj = new GameObject("Headers");
        headerObj.transform.SetParent(contentBox, false);
        RectTransform headerRect = headerObj.AddComponent<RectTransform>();
        headerRect.anchoredPosition = new Vector2(0, 210);
        headerRect.sizeDelta = new Vector2(440, 30);

        HorizontalLayoutGroup headerLayout = headerObj.AddComponent<HorizontalLayoutGroup>();
        headerLayout.childAlignment = TextAnchor.MiddleCenter;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = false;
        headerLayout.padding = new RectOffset(15, 15, 0, 0);
        headerLayout.spacing = 10f;

        CreateHeaderText(headerObj.transform, "RankH", "#", 50, TextAnchor.MiddleCenter);
        CreateHeaderText(headerObj.transform, "NameH", "PLAYER", 200, TextAnchor.MiddleLeft, true);
        CreateHeaderText(headerObj.transform, "ScoreH", "SCORE", 100, TextAnchor.MiddleRight);

        // Scrollable entries container
        RectTransform scrollContent;
        ScrollRect scroll = UIBuilder.CreateScrollView(contentBox, "EntriesScroll", 460, 380, out scrollContent);
        RectTransform scrollRect = scroll.GetComponent<RectTransform>();
        scrollRect.anchoredPosition = new Vector2(0, -30);

        // Store reference to scroll content for refreshing
        scrollContent.gameObject.name = "EntriesContainer";

        // Stats footer
        Text statsText = UIBuilder.CreateText(contentBox, "Stats", "", 16, new Color(0.5f, 0.5f, 0.6f));
        RectTransform statsRect = statsText.GetComponent<RectTransform>();
        statsRect.anchoredPosition = new Vector2(0, -245);
        statsText.alignment = TextAnchor.MiddleCenter;

        Button closeBtn = UIBuilder.CreateButton(contentBox, "CloseButton", "CLOSE (ESC)", 200, 50, UIBuilder.ButtonNormal, () => leaderboardPanel.SetActive(false));
        RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
        closeBtnRect.anchoredPosition = new Vector2(0, -280);

        leaderboardPanel.SetActive(false);
    }

    private void CreateHeaderText(Transform parent, string name, string text, float width, TextAnchor alignment, bool flexible = false)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        Text txt = obj.AddComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 14;
        txt.fontStyle = FontStyle.Bold;
        txt.color = new Color(0.6f, 0.6f, 0.65f);
        txt.alignment = alignment;

        LayoutElement layout = obj.AddComponent<LayoutElement>();
        if (flexible)
        {
            layout.flexibleWidth = 1f;
            layout.minWidth = width;
        }
        else
        {
            layout.minWidth = width;
            layout.preferredWidth = width;
        }
    }

    private void RefreshLeaderboard()
    {
        Transform scrollView = leaderboardPanel.transform.Find("ContentBox/EntriesScroll");
        if (scrollView == null) return;

        Transform entriesContainer = scrollView.Find("Viewport/Content");
        if (entriesContainer == null)
        {
            // Fallback for old structure
            entriesContainer = leaderboardPanel.transform.Find("ContentBox/EntriesContainer");
        }
        if (entriesContainer == null) return;

        // Clear old entries
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        // Update stats text
        Text statsText = leaderboardPanel.transform.Find("ContentBox/Stats")?.GetComponent<Text>();

        // Add entries (now showing all entries, not limited to 10)
        if (LeaderboardManager.Instance != null)
        {
            var entries = LeaderboardManager.Instance.GetLeaderboard();
            int maxEntries = Mathf.Min(entries.Count, 50); // Show up to 50 entries
            
            for (int i = 0; i < maxEntries; i++)
            {
                CreateLeaderboardEntry(entriesContainer, i + 1, entries[i].playerName, entries[i].score);
            }

            if (entries.Count == 0)
            {
                Text emptyText = UIBuilder.CreateText(entriesContainer, "Empty", "No scores yet!\nPlay a game to get on the board!", 24, new Color(0.5f, 0.5f, 0.55f));
            }

            // Update stats
            if (statsText != null)
            {
                if (entries.Count > 0)
                {
                    int totalGames = entries.Count;
                    int avgScore = 0;
                    foreach (var e in entries) avgScore += e.score;
                    avgScore = totalGames > 0 ? avgScore / totalGames : 0;
                    statsText.text = $"Total Games: {totalGames}  •  Average Score: {avgScore:N0}";
                }
                else
                {
                    statsText.text = "";
                }
            }
        }

        // Reset scroll position to top
        ScrollRect scroll = scrollView.GetComponent<ScrollRect>();
        if (scroll != null)
        {
            scroll.verticalNormalizedPosition = 1f;
        }
    }

    private void CreateLeaderboardEntry(Transform parent, int rank, string playerName, int score)
    {
        GameObject entryObj = new GameObject($"Entry_{rank}");
        entryObj.transform.SetParent(parent, false);

        RectTransform rect = entryObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(420, 40);

        Image bg = entryObj.AddComponent<Image>();
        if (rank == 1) bg.color = new Color(1f, 0.84f, 0f, 0.25f);
        else if (rank == 2) bg.color = new Color(0.75f, 0.75f, 0.78f, 0.2f);
        else if (rank == 3) bg.color = new Color(0.8f, 0.5f, 0.2f, 0.18f);
        else bg.color = new Color(0.15f, 0.15f, 0.2f, 0.15f);

        // Use a horizontal layout for proper spacing
        HorizontalLayoutGroup hlg = entryObj.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.padding = new RectOffset(15, 15, 5, 5);
        hlg.spacing = 10f;

        string rankStr = rank == 1 ? "🥇" : rank == 2 ? "🥈" : rank == 3 ? "🥉" : $"{rank}.";
        
        // Rank text (fixed width)
        GameObject rankObj = new GameObject("Rank");
        rankObj.transform.SetParent(entryObj.transform, false);
        Text rankText = rankObj.AddComponent<Text>();
        rankText.text = rankStr;
        rankText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rankText.fontSize = 20;
        rankText.color = Color.white;
        rankText.alignment = TextAnchor.MiddleCenter;
        LayoutElement rankLayout = rankObj.AddComponent<LayoutElement>();
        rankLayout.minWidth = 50;
        rankLayout.preferredWidth = 50;

        // Name text (flexible width)
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(entryObj.transform, false);
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = playerName;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 20;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleLeft;
        LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
        nameLayout.flexibleWidth = 1f;
        nameLayout.minWidth = 150;

        // Score text (fixed width, right aligned)
        GameObject scoreObj = new GameObject("Score");
        scoreObj.transform.SetParent(entryObj.transform, false);
        Text scoreTextEntry = scoreObj.AddComponent<Text>();
        scoreTextEntry.text = score.ToString("N0");
        scoreTextEntry.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreTextEntry.fontSize = 20;
        scoreTextEntry.color = UIBuilder.PrimaryGreen;
        scoreTextEntry.alignment = TextAnchor.MiddleRight;
        LayoutElement scoreLayout = scoreObj.AddComponent<LayoutElement>();
        scoreLayout.minWidth = 100;
        scoreLayout.preferredWidth = 100;
    }

    private void HandleGameOverInput()
    {
        if (leaderboardPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnRestartClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnMainMenuClicked();
        }
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private Color GetDifficultyColor(GameDifficulty diff)
    {
        switch (diff)
        {
            case GameDifficulty.Easy: return UIBuilder.EasyGreen;
            case GameDifficulty.Normal: return UIBuilder.NormalYellow;
            case GameDifficulty.Hard: return UIBuilder.HardRed;
            default: return Color.white;
        }
    }

    /// <summary>
    /// Called by Game when game over occurs
    /// </summary>
    public void ShowGameOver(int finalScore, bool isNewRecord, int rank)
    {
        if (gameOverPanel == null) return;

        gameOverPanel.SetActive(true);

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = $"Score: {finalScore}";
        }

        if (newRecordText != null)
        {
            if (isNewRecord)
            {
                newRecordText.gameObject.SetActive(true);
                if (rank == 1)
                {
                    newRecordText.text = "🏆 NEW HIGH SCORE! 🏆";
                    newRecordText.color = new Color(1f, 0.84f, 0f);
                }
                else
                {
                    newRecordText.text = $"🎉 #{rank} on Leaderboard! 🎉";
                    newRecordText.color = UIBuilder.PrimaryGreen;
                }
            }
            else
            {
                newRecordText.gameObject.SetActive(false);
            }
        }
    }
}
