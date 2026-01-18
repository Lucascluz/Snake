using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Builds and manages the Main Menu UI programmatically
/// Attach this to an empty GameObject in the MainMenu scene
/// </summary>
public class MainMenuBuilder : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameModeSceneName = "GameModeMenu";

    // UI References (created at runtime)
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Text titleText;
    private Text subtitleText;
    private Button playButton;
    private Button leaderboardButton;
    private Button quitButton;
    private GameObject leaderboardPanel;

    // Animation
    private float titleAnimTime = 0f;
    private Vector3 titleOriginalPos;

    private void Start()
    {
        BuildUI();
        StartCoroutine(AnimateEntrance());
    }

    private void Update()
    {
        AnimateTitle();
        HandleInput();
    }

    private void BuildUI()
    {
        // Create canvas
        canvas = UIBuilder.CreateCanvas("MainMenuCanvas");
        canvasGroup = UIBuilder.AddCanvasGroup(canvas.gameObject);
        canvasGroup.alpha = 0f;

        // Background
        UIBuilder.CreatePanel(canvas.transform, "Background", UIBuilder.DarkBackground);

        // Create decorative snake pattern (optional visual flair)
        CreateDecorativeElements();

        // Main content container
        RectTransform contentPanel = UIBuilder.CreateCenteredPanel(canvas.transform, "Content", 800, 600, Color.clear);

        // Title
        titleText = UIBuilder.CreateText(contentPanel, "Title", "SNAKE", 120, UIBuilder.PrimaryGreen);
        titleText.fontStyle = FontStyle.Bold;
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 180);
        titleRect.sizeDelta = new Vector2(600, 140);
        titleOriginalPos = titleRect.anchoredPosition;

        // Subtitle
        subtitleText = UIBuilder.CreateText(contentPanel, "Subtitle", "A Classic Reimagined", 28, new Color(0.6f, 0.6f, 0.65f));
        subtitleText.fontStyle = FontStyle.Italic;
        RectTransform subRect = subtitleText.GetComponent<RectTransform>();
        subRect.anchoredPosition = new Vector2(0, 100);

        // Buttons container
        VerticalLayoutGroup buttonLayout = UIBuilder.CreateVerticalLayout(contentPanel, "Buttons", 20f);
        RectTransform buttonLayoutRect = buttonLayout.GetComponent<RectTransform>();
        buttonLayoutRect.anchoredPosition = new Vector2(0, -50);

        // Play Button
        playButton = UIBuilder.CreateButton(buttonLayout.transform, "PlayButton", "▶  PLAY", 320, 70, UIBuilder.PrimaryGreen, OnPlayClicked);

        // Leaderboard Button
        leaderboardButton = UIBuilder.CreateButton(buttonLayout.transform, "LeaderboardButton", "🏆  LEADERBOARD", 320, 60, UIBuilder.ButtonNormal, OnLeaderboardClicked);

        // Quit Button
        quitButton = UIBuilder.CreateButton(buttonLayout.transform, "QuitButton", "✕  QUIT", 320, 60, new Color(0.5f, 0.2f, 0.2f), OnQuitClicked);

        // Footer
        Text footerText = UIBuilder.CreateText(contentPanel, "Footer", "Press ENTER to Play  •  L for Leaderboard  •  ESC to Quit", 18, new Color(0.4f, 0.4f, 0.45f));
        RectTransform footerRect = footerText.GetComponent<RectTransform>();
        footerRect.anchoredPosition = new Vector2(0, -250);
        footerRect.sizeDelta = new Vector2(800, 30);

        // Build leaderboard panel (hidden by default)
        BuildLeaderboardPanel();
    }

    private void CreateDecorativeElements()
    {
        // Create some decorative squares in the background
        for (int i = 0; i < 15; i++)
        {
            GameObject decorObj = new GameObject($"Decor_{i}");
            decorObj.transform.SetParent(canvas.transform, false);

            RectTransform rect = decorObj.AddComponent<RectTransform>();
            float size = Random.Range(20f, 60f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchorMin = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
            rect.anchorMax = rect.anchorMin;
            rect.anchoredPosition = Vector2.zero;

            Image img = decorObj.AddComponent<Image>();
            img.color = new Color(UIBuilder.PrimaryGreen.r, UIBuilder.PrimaryGreen.g, UIBuilder.PrimaryGreen.b, Random.Range(0.02f, 0.08f));

            // Slow rotation animation
            StartCoroutine(AnimateDecor(rect, Random.Range(-10f, 10f)));
        }
    }

    private IEnumerator AnimateDecor(RectTransform rect, float rotSpeed)
    {
        while (rect != null)
        {
            rect.Rotate(0, 0, rotSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void BuildLeaderboardPanel()
    {
        leaderboardPanel = new GameObject("LeaderboardPanel");
        leaderboardPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = leaderboardPanel.AddComponent<RectTransform>();
        UIBuilder.StretchToFill(panelRect);

        // Semi-transparent background
        Image bg = leaderboardPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.85f);

        // Content box
        RectTransform contentBox = UIBuilder.CreateCenteredPanel(leaderboardPanel.transform, "ContentBox", 480, 580, UIBuilder.PanelBackground);

        // Title
        Text lbTitle = UIBuilder.CreateText(contentBox, "Title", "🏆 LEADERBOARD", 36, UIBuilder.PrimaryGreen);
        RectTransform lbTitleRect = lbTitle.GetComponent<RectTransform>();
        lbTitleRect.anchoredPosition = new Vector2(0, 245);

        // Entries container (simple vertical layout, no scroll)
        GameObject entriesObj = new GameObject("Entries");
        entriesObj.transform.SetParent(contentBox, false);
        RectTransform entriesRect = entriesObj.AddComponent<RectTransform>();
        entriesRect.anchoredPosition = new Vector2(0, 0);
        entriesRect.sizeDelta = new Vector2(420, 400);

        VerticalLayoutGroup entriesLayout = entriesObj.AddComponent<VerticalLayoutGroup>();
        entriesLayout.spacing = 6f;
        entriesLayout.childAlignment = TextAnchor.UpperCenter;
        entriesLayout.childControlWidth = false;
        entriesLayout.childControlHeight = false;
        entriesLayout.childForceExpandWidth = false;
        entriesLayout.childForceExpandHeight = false;
        entriesLayout.padding = new RectOffset(0, 0, 10, 10);

        // Close button
        Button closeBtn = UIBuilder.CreateButton(contentBox, "CloseButton", "CLOSE (ESC)", 200, 50, UIBuilder.ButtonNormal, () => leaderboardPanel.SetActive(false));
        RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
        closeBtnRect.anchoredPosition = new Vector2(0, -250);

        leaderboardPanel.SetActive(false);
    }

    private void PopulateLeaderboardEntries(Transform entriesContainer)
    {
        if (entriesContainer == null) return;

        // Clear old entries
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        // Add entries (max 10)
        if (LeaderboardManager.Instance != null)
        {
            var entries = LeaderboardManager.Instance.GetLeaderboard();
            int maxEntries = Mathf.Min(entries.Count, 10);
            
            for (int i = 0; i < maxEntries; i++)
            {
                CreateLeaderboardEntry(entriesContainer, i + 1, entries[i].playerName, entries[i].score);
            }

            if (entries.Count == 0)
            {
                Text emptyText = UIBuilder.CreateText(entriesContainer, "Empty", "No scores yet!\nBe the first to play!", 24, new Color(0.5f, 0.5f, 0.55f));
            }
        }
        else
        {
            Text emptyText = UIBuilder.CreateText(entriesContainer, "Empty", "No scores yet!\nBe the first to play!", 24, new Color(0.5f, 0.5f, 0.55f));
        }
    }

    private void RefreshLeaderboard()
    {
        Transform entriesContainer = leaderboardPanel.transform.Find("ContentBox/Entries");
        if (entriesContainer == null) return;

        PopulateLeaderboardEntries(entriesContainer);
    }

    private void CreateLeaderboardEntry(Transform parent, int rank, string playerName, int score)
    {
        GameObject entryObj = new GameObject($"Entry_{rank}");
        entryObj.transform.SetParent(parent, false);

        RectTransform rect = entryObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(420, 45);

        Image bg = entryObj.AddComponent<Image>();
        if (rank == 1) bg.color = new Color(1f, 0.84f, 0f, 0.25f);
        else if (rank == 2) bg.color = new Color(0.75f, 0.75f, 0.78f, 0.2f);
        else if (rank == 3) bg.color = new Color(0.8f, 0.5f, 0.2f, 0.18f);
        else bg.color = new Color(0.15f, 0.15f, 0.2f, 0.15f);

        // Use horizontal layout for proper spacing
        HorizontalLayoutGroup hlg = entryObj.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.padding = new RectOffset(15, 15, 5, 5);
        hlg.spacing = 10f;

        // Rank
        string rankStr = rank == 1 ? "🥇" : rank == 2 ? "🥈" : rank == 3 ? "🥉" : rank.ToString();
        GameObject rankObj = new GameObject("Rank");
        rankObj.transform.SetParent(entryObj.transform, false);
        Text rankText = rankObj.AddComponent<Text>();
        rankText.text = rankStr;
        rankText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rankText.fontSize = 22;
        rankText.color = Color.white;
        rankText.alignment = TextAnchor.MiddleCenter;
        LayoutElement rankLayout = rankObj.AddComponent<LayoutElement>();
        rankLayout.minWidth = 50;
        rankLayout.preferredWidth = 50;

        // Name (flexible width)
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(entryObj.transform, false);
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = playerName;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 22;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleLeft;
        LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
        nameLayout.flexibleWidth = 1f;
        nameLayout.minWidth = 150;

        // Score (fixed width, right aligned)
        GameObject scoreObj = new GameObject("Score");
        scoreObj.transform.SetParent(entryObj.transform, false);
        Text scoreText = scoreObj.AddComponent<Text>();
        scoreText.text = score.ToString("N0");
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 22;
        scoreText.color = UIBuilder.PrimaryGreen;
        scoreText.alignment = TextAnchor.MiddleRight;
        LayoutElement scoreLayout = scoreObj.AddComponent<LayoutElement>();
        scoreLayout.minWidth = 100;
        scoreLayout.preferredWidth = 100;
    }

    private IEnumerator AnimateEntrance()
    {
        yield return new WaitForSeconds(0.1f);

        // Fade in
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private void AnimateTitle()
    {
        if (titleText == null) return;

        titleAnimTime += Time.deltaTime;
        float yOffset = Mathf.Sin(titleAnimTime * 1.5f) * 8f;
        
        RectTransform rect = titleText.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(titleOriginalPos.x, titleOriginalPos.y + yOffset);
    }

    private void HandleInput()
    {
        if (leaderboardPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.L))
            {
                leaderboardPanel.SetActive(false);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayClicked();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            OnLeaderboardClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitClicked();
        }
    }

    private void OnPlayClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClickSound();
        StartCoroutine(TransitionToScene(gameModeSceneName));
    }

    private void OnLeaderboardClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuSelectSound();
            
        // Refresh and show leaderboard
        if (leaderboardPanel != null)
        {
            RefreshLeaderboard();
            leaderboardPanel.SetActive(true);
        }
    }

    private void OnQuitClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClickSound();
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
